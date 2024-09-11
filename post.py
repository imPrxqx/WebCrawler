import requests
import random
import string

urlRecords = 'https://localhost:32771/api/Records'
urlNodes = 'https://localhost:32771/api/Nodes'

def random_string(length=10):
    letters = string.ascii_lowercase
    return ''.join(random.choice(letters) for _ in range(length))

def random_tags():
    num_tags = random.randint(0, 5)
    return ', '.join(random_string(5) for _ in range(num_tags))

def generate_random_data_Website():
    return {
        "Url": f"https://{random_string(8)}.com",
        "BoundaryRegExp": random_string(5),
        "Days": random.randint(1, 30),
        "Hours": random.randint(0, 23),
        "Minutes": random.randint(0, 59),
        "Label": random_string(8),
        "IsActive": random.choice([True, False]),
        "Tags": random_tags(),
    }

def generate_random_data_Node():
    return {
        "title": f"https://{random_string(8)}.com",
        "crawlTime": random_string(5),
        "UrlMain": f"https://{random_string(8)}.com",
        "WebsiteRecordId": random.randint(1, 45)
    }

def generate_random_data_NodeNeighbour():
    return {
        "NodeId": random.randint(1, 45),
        "NeighbourNodeId": random.randint(1, 45),
    }
    

for _ in range(50):
    data_Website = generate_random_data_Website()
    response = requests.post(urlRecords, json=data_Website, verify=False)

for _ in range(50):    
    data_Node = generate_random_data_Node()
    response = requests.post(urlNodes, json=data_Node, verify=False)
"""
for _ in range(50):    
    data_NodeNeighbour = generate_random_data_NodeNeighbour()
    response = requests.post(url, json=data_NodeNeighbour, verify=False)
  """  
