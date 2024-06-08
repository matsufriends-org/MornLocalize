# 概要

事前にローカルに落としたスプレッドシートを元に、簡易的な多言語対応ができる自分用ライブラリ

言語及びキー指定により、対応した言語の文字列を取得できる

```csharp
[SerializeField] private MornLocalizeMasterData localizeAsset;
[SerializeField] private Text label;

void Start()
{
    label.text = localizeAsset.Get("jp", "system.yes");
}
```

### 動作確認環境

- Unity 2022.3.14f1

### 依存ライブラリ

- UniTask
    - https://github.com/Cysharp/UniTask
    - 通信の待機に用いている
- MornSpreadSheet
    - https://github.com/matsufriends/MornSpreadSheet
    - スプレッドシートの取得/整形に用いている

# 使い方

- 適当なスプレッドシートを用意する
    - 1行目に、言語を記載する
    - 2行目以降に、キーと言語ごとの対応を記載する

|            | jp  | en  | ... |
|------------|-----|-----|-----|
| system.yes | はい  | Yes | ... |
| system.no  | いいえ | No  | ... |
| ...        | ... | ... | ... |

- スプレッドシートのIDをメモ
    - https://docs.google.com/spreadsheets/d/スプレッドシートのID
- シート名をメモ
    - Sheet1

- Unityに戻り、Project欄を右クリック、`MornLocalize/MornLocalizeMasterData`を作成
    - `SheetId`と`SheetName`にそれぞれメモした値を設定
    - `DefaultLanguage` にデフォルトの言語を設定
    - `Load`ボタンを押す
- `MornLocalizeMasterData` アセットを参照し、言語やキーを指定して対応した文字列を取得する

### その他
- `DefineSymbol` に `DISABLE_MORN_LOCALIZE_LOG` を設定すると、ログ出力を無効化できる