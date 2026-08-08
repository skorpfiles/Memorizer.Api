using System;

namespace SkorpFiles.Memorizer.Api.DataAccess.Tests
{
    /// <summary>
    /// Well-known identifiers and facts about the data produced by
    /// SkorpFiles.Memorizer.Api.DataAccess/Scripts/TestData.sql, so the integration
    /// tests can address specific seeded rows without re-deriving the GUID scheme.
    ///
    /// The script gives every row a fixed GUID grouped by entity type:
    ///   A0000000-…  questionnaires (n = 1..50)
    ///   B0000000-…  questions      (n = global 1..2500, = (questionnaire-1)*50 + code)
    ///   E0000000-…  trainings      (n = 1..12)
    /// and the six users are the standard placeholder logins with digit-repeated GUIDs.
    /// </summary>
    public static class ScriptData
    {
        // Users (log in with the login, the API resolves by user name).
        public static readonly Guid Alice = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid Bob = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static readonly Guid Carol = Guid.Parse("33333333-3333-3333-3333-333333333333");
        public static readonly Guid Dave = Guid.Parse("44444444-4444-4444-4444-444444444444");
        public static readonly Guid Erin = Guid.Parse("55555555-5555-5555-5555-555555555555");
        public static readonly Guid Frank = Guid.Parse("66666666-6666-6666-6666-666666666666");

        /// <summary>50 questionnaires per run, 50 questions each, 68 removed typed answers, etc.</summary>
        public const int QuestionnairesCount = 50;
        public const int QuestionsPerQuestionnaire = 50;

        /// <summary>Questionnaire GUID for questionnaire number <paramref name="number"/> (1..50).</summary>
        public static Guid QuestionnaireId(int number) => Make('A', number);

        /// <summary>Global question GUID; <paramref name="globalNumber"/> = (questionnaire-1)*50 + code.</summary>
        public static Guid QuestionId(int globalNumber) => Make('B', globalNumber);

        /// <summary>Question GUID from its questionnaire number and 1-based code within it.</summary>
        public static Guid QuestionId(int questionnaireNumber, int codeInQuestionnaire) =>
            Make('B', (questionnaireNumber - 1) * QuestionsPerQuestionnaire + codeInQuestionnaire);

        /// <summary>Training GUID for training number <paramref name="number"/> (1..12).</summary>
        public static Guid TrainingId(int number) => Make('E', number);

        // --- Specific rows used by the tests (see the script header for the full index) ---

        /// <summary>Questionnaire 1 "Spanish: food and drink" — private, owned by alice, not removed.</summary>
        public static Guid AlicePrivateQuestionnaire => QuestionnaireId(1);

        /// <summary>Questionnaire 2 "Spanish: everyday verbs" — public, owned by alice, not removed.</summary>
        public static Guid AlicePublicQuestionnaire => QuestionnaireId(2);

        /// <summary>Questionnaire 3 "French: everyday nouns" — public, owned by alice, removed.</summary>
        public static Guid AliceRemovedQuestionnaire => QuestionnaireId(3);

        /// <summary>Questionnaire 15 "Docker commands" — private, owned by bob (foreign to alice).</summary>
        public static Guid BobPrivateQuestionnaire => QuestionnaireId(15);

        /// <summary>Questionnaire 9 "Git commands" — public, owned by bob.</summary>
        public static Guid BobPublicQuestionnaire => QuestionnaireId(9);

        /// <summary>Training 1 "alice's training 1 (by time)" — owned by alice, not removed.</summary>
        public static Guid AliceTraining => TrainingId(1);

        /// <summary>Training 4 "dave's training 4" — removed.</summary>
        public static Guid RemovedTraining => TrainingId(4);

        private static Guid Make(char prefix, int number) =>
            Guid.Parse($"{prefix}0000000-0000-0000-0000-{number:D12}");
    }
}
