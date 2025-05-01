WITH First100Trainings AS (
    SELECT
        t.TrainingResultQuestionId,
        t.TrainingResultTimeSeconds,
        ROW_NUMBER() OVER (PARTITION BY t.TrainingResultQuestionId ORDER BY t.TrainingResultId) AS rn
    FROM memorizer.jTrainingResult t
    WHERE t.TrainingResultUserId = @ownerId
),
FilteredTrainings AS (
    SELECT *
    FROM First100Trainings
    WHERE rn <= 100
),
TrainingStatsWithWindow AS (
    SELECT
        ft.TrainingResultQuestionId,
        COUNT(*) OVER (PARTITION BY ft.TrainingResultQuestionId) AS TrainingCount,
        PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY ft.TrainingResultTimeSeconds)
            OVER (PARTITION BY ft.TrainingResultQuestionId) AS MedianTrainingTime
    FROM FilteredTrainings ft
),
TrainingStats AS (
    SELECT DISTINCT
        TrainingResultQuestionId,
        TrainingCount,
        MedianTrainingTime
    FROM TrainingStatsWithWindow
)
SELECT
    q.QuestionId AS Id,
    0 AS Code,
    q.QuestionType,
    q.QuestionText,
    q.QuestionUntypedAnswer,
    q.QuestionIsEnabled,
    q.QuestionReference,
    q.QuestionEstimatedTrainingTimeSeconds,
    q.ObjectCreationTime AS CreationTimeUtc,
    q.ObjectIsRemoved AS IsRemoved,
    q.ObjectRemovalTime AS RemovalTimeUtc,
    (
        SELECT TypedAnswerId, TypedAnswerText, ObjectCreationTime
        FROM memorizer.rTypedAnswer ta
        WHERE ta.QuestionId = q.QuestionId AND ta.ObjectIsRemoved = 0
        FOR JSON PATH
    ) AS TypedAnswersJson,
    qr.QuestionnaireId,
    qr.QuestionnaireName,
    qu.QuestionUserIsNew,
    qu.QuestionUserRating,
    qu.QuestionUserPenaltyPoints,
    CASE 
        WHEN ts.TrainingCount IS NULL THEN q.QuestionEstimatedTrainingTimeSeconds
        WHEN ts.TrainingCount BETWEEN 1 AND 10 THEN 
            CAST(((q.QuestionEstimatedTrainingTimeSeconds + ts.MedianTrainingTime) / 2.0) AS int)
        ELSE CAST(ts.MedianTrainingTime AS int)
    END AS QuestionActualTrainingTimeSeconds
FROM memorizer.rQuestion q
LEFT JOIN TrainingStats ts ON ts.TrainingResultQuestionId = q.QuestionId
LEFT JOIN memorizer.rQuestionnaire qr ON qr.QuestionnaireId = q.QuestionnaireId
LEFT JOIN memorizer.nnQuestionUser qu ON qu.QuestionId = q.QuestionId AND qu.UserId = @ownerId
WHERE q.ObjectIsRemoved = 0 AND q.QuestionnaireId IN ({inClause});