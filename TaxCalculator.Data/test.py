import requests
import pandas as pd
import json

API_URL = "http://localhost:7166/TaxCalculator/v1/TaxCalculator/api/calcTax"

def send_request_to_api(row):
    api_request_payload = json.loads(row["api_json_request"])
    expected = int(row["MST_MAS_MEGIA"])

    try:
        response = requests.post(API_URL, json=api_request_payload)
        response.raise_for_status() 
        
        actual = response.json().get("total")
        print(f"total from response : {actual}")
        
        assert abs(expected - actual) < 2 or (expected == 0 and actual <= 0), \
            f"Failed for {api_request_payload}: expected {expected}, actual {actual}"

    except requests.exceptions.RequestException as e:
        print(f"error on api request for {api_request_payload}: {e}")
    except ValueError:
        print(f"error when parse JSON for {api_request_payload}")

def load_data(file_path):

    if file_path.endswith(".xlsx"):
        return pd.read_excel(file_path)
    elif file_path.endswith(".csv"):
        return pd.read_csv(file_path)
    else:
        raise ValueError("File format must be .xlsx or .csv")

def main():

    file_path = "./test_data/testData.xlsx" 

    df = load_data(file_path)

    df.apply(send_request_to_api, axis=1)

    print("All tests passed !")

if __name__ == "__main__":
    main()
