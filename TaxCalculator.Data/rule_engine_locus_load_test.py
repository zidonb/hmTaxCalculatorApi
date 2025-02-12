import pandas as pd
from locust import HttpUser, TaskSet, task, between
import json
import logging


file_path = "./test_data/testdata.csv"


def load_data(file_path):
    if file_path.endswith(".xlsx"):
        return pd.read_excel(file_path)
    elif file_path.endswith(".csv"):
        return pd.read_csv(file_path)
    else:
        raise ValueError("File format must be .xlsx or .csv")


def load_requests():
    try:
        df = load_data(file_path)
        api_requests = df["api_json_request"]

        return api_requests

    except FileNotFoundError:
        logging.error(f"no file found at {file_path}")
        return []

    except KeyError:
        logging.error(f"no key 'api_json_request' found ")
        return []


class RuleEngineTaskSet(TaskSet):
    def on_start(self):
        self.requests = load_requests()
        self.message_index = 0

    @task
    def send_request(self):
        if len(self.requests) == 0:
            logging.error("no requests founds")
            return

        message = self.requests[self.message_index]

        payload = json.loads(message)

        with self.client.post(
            "/TaxCalculator/v1/TaxCalculator/api/calcTax",  # Endpoint path
            json=payload,
            catch_response=True,
        ) as response:
            if response.status_code == 200:
                try:
                    response_data = response.json()
                    if "resultObject" in response_data:
                        pass
                    else:
                        response.failure("Expected 'resultObject' in response.")
                except ValueError:
                    response.failure("Response content is not valid JSON.")
            else:
                failed_message = message
                message_index = self.message_index

                try:
                    response_content = response.text

                except Exception as e:
                    response_content = f"Error retrieving response content: {str(e)}"

                failure_message = (
                    f"Received unexpected status code {response.status_code} "
                    f"for message index {message_index}: '{failed_message}'. "
                    f"Response Content: {response_content}"
                )

                response.failure(failure_message)

        self.message_index += 1

        if self.message_index >= len(self.requests):
            self.message_index = 0


class RuleEngineUser(HttpUser):

    tasks = [RuleEngineTaskSet]
    wait_time = between(0, 0.1)
    host = "http://10.83.10.97:8085"  # Base URL without the endpoint path
