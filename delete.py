import requests

url = 'https://localhost:32776/api/Records'

def delete_record(record_id):
    delete_url = f"{url}/{record_id}"
    
    try:
        response = requests.delete(delete_url, verify=False)

        if response.status_code == 200 or response.status_code == 204:
            print(f"Record with Id {record_id} deleted successfully.")
        else:
            print(f"Failed to delete record with Id {record_id}. Status code: {response.status_code}, Response: {response.text}")
    except requests.exceptions.RequestException as e:
        print(f"Request failed: {e}")


ids_to_delete = [9, 10]


for record_id in ids_to_delete:
    delete_record(record_id)
