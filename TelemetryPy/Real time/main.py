from fastapi import FastAPI, WebSocket, WebSocketDisconnect
from datetime import datetime
import json
import matplotlib.pyplot as plt
import pandas as pd
import numpy as np
import time

app = FastAPI()

@app.websocket("/ws")
async def websocket_endpoint(websocket: WebSocket):
    
    dataDict = dict()
    
    await websocket.accept()
    
    try:
        while True:
            data = await websocket.receive_text()

            parsed = json.loads(data)

            print(f"Received barrel data: {parsed}")
            for barrel in parsed["barrels"]:
                name = barrel["name"]
                generatedAt = barrel["generatedAt"]
                destroyedAt = barrel["destroyedAt"]
                
                dataDict[name] = (generatedAt, destroyedAt)
                
                print(barrel)

            await websocket.send_text("Positions received")
            
    except WebSocketDisconnect:
        
        print(dataDict)
        
        barrel_names = []
        lifetimes_ms = []
        
        fmt = "%H:%M:%S.%f"
        cur_time = datetime.now().strftime(fmt)
        
        for name, (gen, des) in dataDict.items():
            if gen:
                t_gen = datetime.strptime(gen, fmt)
                if not des:
                    des = cur_time
                t_des = datetime.strptime(des, fmt)
                lifetime = (t_des - t_gen).total_seconds() * 1000
                barrel_names.append(name)
                lifetimes_ms.append(lifetime)
        
        plt.bar(barrel_names, lifetimes_ms)
        plt.xlabel("Barrel")
        plt.ylabel("Lifetime (ms)")
        plt.title("Barrel Lifetimes")
        plt.xticks(rotation=45)
        plt.tight_layout()
        plt.show()