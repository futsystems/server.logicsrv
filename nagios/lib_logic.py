# -*- coding: utf-8 -*-

import json
import requests

def demo_request():
    response = requests.post(
        "http://127.0.0.1:9999/api/",
        data={"method": "status",})

    return response.status_code , response.json()['Payload']


if __name__ == '__main__':
    v1,v2 = demo_request()

    print v1
    print v2
