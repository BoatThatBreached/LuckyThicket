import pandas as pd
import requests as re

beavers = pd.read_excel("Cards.xlsx", sheet_name="Beavers")
magpies = pd.read_excel("Cards.xlsx", sheet_name="Magpies")

rarityDict = {"легендарная": 3, "эпическая": 2, "редкая": 1, "обычная": 0}

except_indexes = [2, 13, 19, 20, 21, 22, 34, 36, 37, 38, 44, 45, 46, 49]

server_url = "http://a0677209.xsph.ru/test.php"

for index, row in beavers.iterrows():
    ind = row
    if int(ind[3]) not in except_indexes:
        # s = "{" + "\"Id\": {0}, \"Name\": {1}, \"Rarity\": {2}, \"AbilityMask\": {3}, \"AbilityString\": {4}" \
        #      .format(int(ind[3]), str(ind[0]), int(rarityDict[ind[1]]), str(ind[2]), str(ind[4])) + "}"
        if (str(ind[2]) == 'nan'):
            ind[2] = ""
        s = {"Id": int(ind[3]), "Name": ind[0], "Rarity": int(rarityDict[ind[1]]), "AbilityMask": str(ind[2]),
             "AbilityString": str(ind[4])}

##        empty = "{" + "\"Id\": {0}, \"Name\": {1}, \"Rarity\": {2}, \"AbilityMask\": {3}, \"AbilityString\": {4}" \
##            .format(int(ind[3]), "", 0, "", "") + "}"
        print(s)
        response = re.post(server_url, json=s, headers={"Content-type": "application/json; charset=utf-8"})

for index, row in magpies.iterrows():
    ind = row
    if int(ind[3]) not in except_indexes:
        if (str(ind[2]) == 'nan'):
            ind[2] = ""
        s = {"Id": int(ind[3]), "Name": str(ind[0]), "Rarity": int(rarityDict[ind[1]]),
             "AbilityMask": str(ind[2]),
             "AbilityString": str(ind[4])}

##        empty = "{" + "\"Id\": {0}, \"Name\": {1}, \"Rarity\": {2}, \"AbilityMask\": {3}, \"AbilityString\": {4}" \
##            .format(int(ind[3]), "", 0, "", "") + "}"
        print(s)
        response = re.post(server_url, json=s)
