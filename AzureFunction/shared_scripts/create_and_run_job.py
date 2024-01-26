from azureml.core import Workspace, Experiment, Environment, ScriptRunConfig
from azureml.core.authentication import ServicePrincipalAuthentication
from azureml.core.compute import AmlCompute
import logging
import time
import os


def start_compute_target(compute_target):
    try:
        print(f"Checking if compute instance '{compute_target.name}' exists.")
        # If the compute target is not running, start it
        if compute_target.get_status().state != "Running":
            compute_target.start(show_output=True)
            logging.info(
                f"Compute instance '{compute_target.name}' is starting...")

            # Wait for the compute target to be in the "Running" state
            while compute_target.get_status().state != "Running":
                logging.info(
                    f"Waiting for compute instance '{compute_target.name}' to be in 'Running' state...")
                time.sleep(30)  # Adjust the sleep interval as needed

            logging.info(
                f"Compute instance '{compute_target.name}' is now running.")
        else:
            logging.info(
                f"Compute instance '{compute_target.name}' is already running.")
    except Exception as e:
        logging.info(f"An error occurred: {str(e)}")
        raise


def submit_experiment(experiment, script_config):
    # Submit the experiment
    logging.info(f"Submitting job to '{experiment.name}' experiment")
    experiment.submit(config=script_config)
    logging.info(f"Done")


def submit_script(script_name):
    tenant_id = "c66b6765-b794-4a2b-84ed-845b341c086a"
    service_principal_id = "44092769-7233-44f3-9878-fbda9b49b0fc"
    service_principal_password = "rOS8Q~2OQTvfm6a4He.0sewNbWs1KggVkqI-PaVA"

    # Load the Azure ML workspace using service principal authentication
    svc_pr = ServicePrincipalAuthentication(
        tenant_id=tenant_id,
        service_principal_id=service_principal_id,
        service_principal_password=service_principal_password
    )

    workspace = Workspace.get(
        name="sea-slug-wp",
        subscription_id="7c5d4dd1-4815-4ab4-8e58-52b007be1b9b",
        resource_group="sea-slugs-rg",
        auth=svc_pr
    )

    # Create or retrieve an experiment
    experiment = Experiment(workspace, "AI-training")

    # Specify the compute target using the system-assigned managed identity
    compute_target_name = "sea-slug-compute"

    # Create compute target object
    compute_target = AmlCompute(workspace, compute_target_name)

    # Start or check the status of the compute target
    start_compute_target(compute_target)

    # Retrieve the existing environment
    custom_environment_name = "sea-slug-env"
    custom_environment = Environment.get(workspace, custom_environment_name)
    logging.info(f"Custom Environment '{custom_environment.name}' found.")

    # Define the script run configuration for the specified script
    script_config = ScriptRunConfig(
        source_directory=os.path.dirname(os.path.abspath(__file__)),
        script=script_name,
        compute_target=compute_target,
        environment=custom_environment
    )

    # Submit the experiment for the specified script
    submit_experiment(experiment, script_config)
