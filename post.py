import requests
import random
import string

url = 'https://localhost:32774/api/Records'

def random_string(length=10):
    letters = string.ascii_lowercase
    return ''.join(random.choice(letters) for _ in range(length))

def random_tags():
    num_tags = random.randint(0, 5)
    return ', '.join(random_string(5) for _ in range(num_tags))

def generate_random_data():
    return {
        "Url": f"https://{random_string(8)}.com",
        "BoundaryRegExp": random_string(5),
        "Days": random.randint(1, 30),
        "Hours": random.randint(0, 23),
        "Minutes": random.randint(0, 59),
        "Label": random_string(8),
        "IsActive": random.choice([True, False]),
        "Tags": random_tags(),
        "Nodes": [] 
    }

for _ in range(10):
    data = generate_random_data()
    response = requests.post(url, json=data, verify=False)
    print(f"Status Code: {response.status_code}")
    print(f"Response Body: {response.text}")
