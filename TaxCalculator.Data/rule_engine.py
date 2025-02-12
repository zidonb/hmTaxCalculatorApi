import requests
import json
import pandas as pd
from utils import calculate_age

API_URL = "http://10.83.10.97:8085/TaxCalculator/v1/TaxCalculator/api/calcTax"

DEBUG_MODE = False


def get_total_from_rule_engine(row):
    response = requests.post(API_URL, json=row.to_dict())
    if DEBUG_MODE:
        print(json.dumps(row.to_dict()))
    if response.status_code == 200:
        response_dict = response.json()
        response_dict["json_request"] = json.dumps(row.to_dict())
        return response_dict
    else:
        return {
            "deduction": 0,
            "tax": 0,
            "credit": 0,
            "total": 0,
            "resultObject": "",
            "groupedObject": "",
            "resultGroupedObject": "",
            "json_request": json.dumps(row.to_dict()),
        }


def move_columns(df):
    col_to_move_6 = df.columns[5]
    col_to_move_28 = df.columns[27]

    all_cols = list(df.columns)

    all_cols.remove(col_to_move_6)
    all_cols.remove(col_to_move_28)

    all_cols.insert((1), col_to_move_6)
    all_cols.insert(2, col_to_move_28)

    df = df[all_cols]

    return df


def insert_other_columns(df):
    col_masMagia = "MST_MAS_MEGIA"
    col_api_total = "api_total"

    df["Diff"] = df[col_masMagia] - df[col_api_total]

    df["Relevant_To_Check"] = df.apply(
        lambda row: 0
        if (
            abs(row[col_masMagia] - row[col_api_total]) < 2
            or (row[col_masMagia] == 0 and row[col_api_total] <= 0)
        )
        else 1,
        axis=1,
    )

    cols = list(df.columns)
    cols.insert(4, cols.pop(cols.index("Diff")))
    cols.insert(5, cols.pop(cols.index("Relevant_To_Check")))
    print(cols)
    df = df[cols]

    return df


def consume_api(df, OUTPUT_CSV_PATH):
    df["endOfMilitaryServiceDate"] = df["endOfMilitaryServiceDate"].fillna("")
    df["childrensData"] = df["childrensData"].fillna("")
    df["individualAge"] = df["individualAge"].apply(lambda x: calculate_age(str(x)))

    df = df.join(
        df.apply(
            lambda row: pd.Series(get_total_from_rule_engine(row)), axis=1
        ).add_prefix("api_")
    )

    df["endOfMilitaryServiceDate"] = df["endOfMilitaryServiceDate"].apply(
        lambda x: "0" + str(x) if x != "" and int(x) < 99999 else x
    )
    df["endOfMilitaryServiceDate"] = df["endOfMilitaryServiceDate"].astype(str)

    df = move_columns(df)
    df = insert_other_columns(df)
    df.to_csv(OUTPUT_CSV_PATH, index=False, encoding="utf-8-sig")
    return df


if __name__ == "__main__":
    CSV_FILE_PATH = "./merged_output_demo.csv"
    OUTPUT_CSV_PATH = "./ResultsRun.csv"
    consume_api(CSV_FILE_PATH, OUTPUT_CSV_PATH)
