import pandas as pd
from os import path

DEBUG_MODE = True


unwanted_cols = [14, 34, 40, 42, 186, 195, 238, 263, 277, 278, 294, 333, 365]

rename_cols = {
    "MST_TIK": "MST_TIK",
    "MST_MIN1": "isWoman",
    "year": "year",
    "properAccountingBooks": "properAccountingBooks",
    "individual": "individual",
    "MST_MAS_MEGIA": "MST_MAS_MEGIA",
    "MST_TAR_LEDA1": "individualAge",
    20: "israeliResident",
    23: "numberOfChildrenWithDisabilities",
    24: "regularServiceMonths",
    36: "lifeInsurence",  # FIX typo: "lifeInsurance"
    37: "donationAmount",
    45: "pensionInsuranceEmployedDeposit",
    112: "wrdInsuranceSelfEmployedDeposit",
    # 132: "amountPaidForCare",
    135: "pensionInsuranceSelfEmployedDeposit",
    # 140: "survivorPensionInsurance",
    206: "wrdInsuranceEmployedDeposit",
    224: "endOfMilitaryServiceDate",
    244: "insuredIncome",
    248: "pensionInsuranceFromEmployerDeposit",
    260: "childrensData",
    "personalExertionIncome": "personalExertionIncome",
}


def save_statistics(df1):
    if DEBUG_MODE:
        c = df1["CODE"].value_counts()
        c = c.drop(
            unwanted_cols + list(rename_cols.keys()) + [158, 150, 258], errors="ignore"
        )

        c.to_csv("./out_data/counts.csv", index=True, encoding="utf-8-sig")


def make_dataset(
    data_folder,
    data_fields_file="nigrarot_large.xlsx",
    data_file="37878_from_shuma_mast.xlsx",
):
    # Load the first sheet (CODE, VALUE, MST_TIK)
    df1 = pd.read_excel(path.join(data_folder, data_fields_file))
    save_statistics(df1)

    # Load the second sheet (MST_TIK, CODE20, CODE21, etc.)
    df2 = pd.read_excel(path.join(data_folder, data_file))

    # Pivot the first dataframe (df1) to make columns for each CODE-VALUE pair
    df1_pivoted = df1.pivot_table(
        index="MST_TIK", columns="CODE", values="SCHUM", aggfunc="first"
    )

    # Now merge this pivoted dataframe with the second dataframe on MST_TIK
    df_merged = pd.merge(df2, df1_pivoted, on="MST_TIK", how="left")
    df_merged.to_csv("./out_data/dfMerged.csv", index=True, encoding="utf-8-sig")
    return df_merged


def step1_remove_cols(df):
    for col in rename_cols.keys():
        if col not in df.columns:
            df[col] = ""
    df = df.drop(unwanted_cols, axis=1)
    
    return df


def step2_join_cols(df):
    df["personalExertionIncome"] = (
        df[150].fillna(0) + df[158].fillna(0) + df[258].fillna(0)
    )
    df = df.drop([150, 158, 258], axis=1)

    return df


def step3_hardcoded_cols(df):
    """Field that we dont have from the data"""
    df["individual"] = True
    df["properAccountingBooks"] = True
    df["year"] = 2022
    return df


def step4_transofrm_types(df):
    df["MST_MIN1"] = df["MST_MIN1"] == 2

    df[20] = df[20].replace({0: True})
    df[20] = df[20].fillna(False)
    return df


def step5_rename_cols(df):
    df = df.rename(columns=rename_cols)
    return df


def step6_remove_unwanted_cols(df):
    out_cols = [col for col in df.columns if col not in rename_cols.values()]

    if DEBUG_MODE:
        print('-' * 10)
        print(out_cols)

    df = df[~df[out_cols].notna().any(axis=1)]

    df = df[rename_cols.values()]

    return df


def step7_set_na_values(df):
    """we want 0 to be written in the cell instead of null in the file"""

    na_cols = [col for col in df.columns if col not in ["childrensData"]]

    df[na_cols] = df[na_cols].fillna(0)

    return df


def step8_add_unmapped_cols(df):
    """Field that must exist and we didnt added yet"""
    df["taxableIncomeNotFromPersonalExertion"] = 0.0
    df["pensionInsuranceSelfEmployedDeposit"] = 0.0
    df["survivorPensionInsurance"] = 0.0
    df["wrdInsuranceSelfEmployedDeposit"] = 0.0
    df["amountPaidForCare"] = 0.0
    return df


def step9_data_format_cols(df):
    df["endOfMilitaryServiceDate"] = (
        df["endOfMilitaryServiceDate"].astype(int).astype(str)
    )

    df["endOfMilitaryServiceDate"] = df["endOfMilitaryServiceDate"].str.zfill(6)

    df["endOfMilitaryServiceDate"] = df["endOfMilitaryServiceDate"].replace(
        {"0" * 6: ""}
    )
    """
    df["endOfMilitaryServiceDate"] = df["endOfMilitaryServiceDate"].where(
        df["endOfMilitaryServiceDate"] != "",
        df["endOfMilitaryServiceDate"].str.zfill(9),
    )
    """
    if DEBUG_MODE:
        print(df["endOfMilitaryServiceDate"])
    return df


def step10_set_datatype_to_cols(df):
    df["childrensData"] = df["childrensData"].fillna("").astype(str)

    df["year"] = df["year"].astype(int)

    df["individualAge"] = df["individualAge"].astype(int)

    df["numberOfChildrenWithDisabilities"] = df[
        "numberOfChildrenWithDisabilities"
    ].astype(int)

    return df


def prepare_dataset(data_folder, output_path):
    df = make_dataset(data_folder)
    print(len(df))
    df = step1_remove_cols(df)
    print(len(df))
    df = step2_join_cols(df)
    print(len(df))
    df = step3_hardcoded_cols(df)
    print(len(df))
    df = step4_transofrm_types(df)
    print(len(df))
    df = step5_rename_cols(df)
    print(len(df))
    df = step6_remove_unwanted_cols(df)
    print(len(df))
    df = step7_set_na_values(df)
    print(len(df))
    df = step8_add_unmapped_cols(df)
    print(len(df))
    df = step9_data_format_cols(df)
    print(len(df))
    df = step10_set_datatype_to_cols(df)
    print(len(df))

    print(f"Dataset have total of {len(df)} rows")

    df.to_csv(output_path, index=False, encoding="utf-8-sig")

    return df


if __name__ == "__main__":
    prepare_dataset()
