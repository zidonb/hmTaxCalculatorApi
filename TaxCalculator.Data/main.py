from dataset import prepare_dataset
from rule_engine import consume_api
from os import path, makedirs


def test_single(df, MST_TIK):
    df = df[df["MST_TIK"] == MST_TIK]
    return df


if __name__ == "__main__":
    TEST_MST_TIK = None  # 320674450
    data_folder = (
        "G:/חממה/projects/2025 Q1 Tax Simulator Pilote/sprint 4"  # "./data"
    )

    output_path = "./out_data"
    output_path_df = path.join(output_path, "merged_output.csv")
    output_path_results = path.join(output_path, "ResultsRun.csv")
    makedirs(output_path, exist_ok=True)

    df = prepare_dataset(data_folder=data_folder, output_path=output_path_df)

    if TEST_MST_TIK is not None:
        df = test_single(df, TEST_MST_TIK)

    consume_api(df, output_path_results)
