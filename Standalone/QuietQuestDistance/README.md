# Quiet Quest Distance

7 Days to Die V3.2向けの単独導入Modです。画面内のクエスト目的地マーカーから距離文字列だけを隠し、マーカーアイコンとクエスト進行表示は残します。

## 対象

- quest
- rally
- go_to_trader
- return_to_trader
- quest_switch

一般ウェイポイント、仲間、補給物資など、クエスト以外の距離表示には触れません。

## 操作

- `F8`: 距離表示／非表示を切り替え
- `qqd hide`: 非表示
- `qqd show`: 表示
- `qqd toggle`: 切り替え
- `qqd status`: 現在値を確認
- `qqd reload`: 設定ファイルを再読込

切り替え状態は再起動後も保持されます。キーは `Config/quiet-quest-distance.json` の `toggleKey` で変更できます。

## 仕組み

対象5種の `NavObjectScreenSettings.ShowTextType` だけを `Distance` と `None` の間で切り替えます。Harmonyによる描画パッチは使いません。ゲーム本来のマーカー描画を維持したまま、距離ラベルだけを止めます。

## ビルド

PowerShellで実行します。

```powershell
.\build.ps1
```

別の場所へインストールしている場合:

```powershell
.\build.ps1 -GameDir "D:\SteamLibrary\steamapps\common\7 Days To Die"
```

## 導入

ビルド後、`QuietQuestDistance` フォルダ全体を7DTDの `Mods` 配下へコピーします。DLL ModのためEACを無効にしてください。
