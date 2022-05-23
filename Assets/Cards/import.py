import pandas as pd
import requests as re

beavers = pd.read_excel("Cards.xlsx", sheet_name="Beavers")
magpies = pd.read_excel("Cards.xlsx", sheet_name="Magpies")

rarityDict = {"легендарная": 3, "эпическая": 2, "редкая": 1, "обычная": 0}

except_indexes = [13, 19, 21, 22, 34, 36, 37, 38, 44, 45, 46, 49]

for index, row in beavers.iterrows():
    ind = row
    if int(ind[3]) not in except_indexes:
        s = {"Id": int(ind[3]), "Name": str(ind[0]), "Rarity": int(rarityDict[ind[1]]), "AbilityMask": str(ind[2]),
             "AbilityString": str(ind[4])}

        response = re.post("http://a0664388.xsph.ru/test.php", json=s, headers={'Content-type': 'text/plain'
                                                                                'charset=utf-8'})

for index, row in beavers.iterrows():
    ind = row
    if int(ind[3]) not in except_indexes:
        s = {"Id": int(ind[3]), "Name": str(ind[0]), "Rarity": int(rarityDict[ind[1]]), "AbilityMask": str(ind[2]),
             "AbilityString": str(ind[4])}

        response = re.post("http://a0664388.xsph.ru/test.php", json=s, headers={'Content-type': 'text/plain; '
                                                                                                'charset=utf-8'})
