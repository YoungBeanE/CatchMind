import requests
import json
import csv

def popul_keyword_list(popul_cnt):
    url = f'https://www.11st.co.kr/AutoCompleteAjaxAction.tmall?method=getKeywordRankJson&type=hot&isSSL=Y&rankCnt={popul_cnt}&callback=fetchSearchRanking'
    headers = {
        'user-agent': 'Chrome'
        # 'user-agent': 'Mozilla/5.0 (Windows NT 10.0; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36'
    }
    response = requests.get(url, headers=headers) #요청

    if response.status_code == 200:
        response.encoding = 'utf-8'
        response_str = response.text

        fr = response_str.find('({')
        to = response_str.find('})')

        response_str = response_str[fr + 1 : to + 1] # { ~ } 부분 추출
        dict_data = json.loads(response_str)
        print(dict_data)
        print(dict_data['items']) # 'items'추출
        
        #파일명, 쓰기모드, 인코딩, 줄생성방지옵션
        with open('KeyWord.csv', 'w', encoding='utf-8-sig', newline='') as f:
            writer = csv.writer(f)

            items = []
            for item in dict_data['items']:
                items += {f'{item["keyword"]}'} # {}로 감싸지 않으면 한글자씩 쪼개짐
            
            writer.writerow(items)

popul_keyword_list(200)

print("실행됨")