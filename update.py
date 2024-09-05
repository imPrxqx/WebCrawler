import requests


url = 'https://localhost:32776/api/Records'
record_id = 11  


model = {
    "Url": "http://example.com",                
    "BoundaryRegExp": ".*example.*",           
    "Days": 7,                                  
    "Hours": 12,                                
    "Minutes": 30,                              
    "Label": "Example Label",                   
    "IsActive": True,                           
    "Tags": "example, test"                     
}


update_url = f"{url}/{record_id}"


try:
    response = requests.put(update_url, json=model, verify=False)

    if response.status_code == 200:
        print(f"Record with ID {record_id} updated successfully.")
    else:
        print(f"Failed to update the record: {response.status_code} - {response.text}")

except Exception as e:
    print(f"An error occurred: {e}")
