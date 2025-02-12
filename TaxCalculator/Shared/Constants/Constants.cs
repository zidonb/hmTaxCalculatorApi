namespace TaxCalculator.Shared.Constants;

public class Constants {
    #region Endpoints

    public const string TAX_CALCULATOR_GROUP = "TaxCalculator";

    #endregion

    #region Redis

    public const string REDIS_KEY_SAMPLES = "samples";

    #endregion

    #region Worflows

    public const string WORKFLOW_DIRECTORY_NAME = "Config";
    public const string DEFAULT_DIRECTORY_NAME = "default";
    public const string SUMMARY_WORKFLOW_NAME = "summary";
    public const string DEPENDENCY_GLOBAL_PARAM_NAME = "dependency";
    public const string GLOBAL_PARAMETERS_CSV_FILE_NAME = "globalParametersDB.csv";
    public const string MASKING_DIRECTORY = "masking";
    #endregion

}