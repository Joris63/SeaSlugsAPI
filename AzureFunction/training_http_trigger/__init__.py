import azure.functions as func
import logging
from shared_scripts.create_and_run_job import submit_script
import sys
import os

sys.path.append(os.path.abspath("../"))


def main(req: func.HttpRequest) -> func.HttpResponse:
    try:
        logging.info(f"Submit script.")
        submit_script("model_training_script.py")
        return func.HttpResponse(f"Script submitted.", status_code=200)
    except Exception as e:
        # Log the error
        logging.error(f"An error occurred: {str(e)}.")

        # Return an HTTP response indicating the error
        return func.HttpResponse(f"An error occurred: {str(e)}.", status_code=500)