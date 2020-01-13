![](https://user-images.githubusercontent.com/31283418/72233483-fa971680-360a-11ea-9491-6e2f8932ecfd.png)
# Voiceer
Voice+Cheer=Voiceer(ぼいしあ)

かわいい声でコーディングを応援してくれるUnity拡張です。

開発協力：[結目ユイ様](https://twitter.com/musubimeyui)

## これは何？
Unityで

* セーブしたとき
* 再生したとき
* 再生を終了した時
* ビルドした時
…など、あらゆる状態変化をフックし、事前に登録した音声を再生するEditor拡張です。

デモ動画：
https://twitter.com/CST_negi/status/1214500926326628352

## 動作環境
Unity2018.4.5または4.15で動作チェックしています。

(※Unity2019や2020ではEditor側のAPIが変更されている可能性があるため動作未確認です。)

## 使い方
1. [Release](https://github.com/negipoyoc/Voiceer/releases)より、Voiceer+Sample.unitypackageをダウンロードします。
2. プロジェクトにインポートします。
3. 終わりです。これでボイスが再生されます。

## 違うボイスPresetを試したい時
1.Window->Voiceer->Voice Preset Selectorを選択します。
![](https://user-images.githubusercontent.com/31283418/72231862-08e03500-3601-11ea-9a1b-f9eadd6d99a7.png)

2.赤丸の部分をクリックし、Presetを選択します。
![](https://user-images.githubusercontent.com/31283418/72231936-7ee49c00-3601-11ea-9c0b-b7da798ce87d.png)

3.終わりです。

## ボイスの種類を自分で編集したい時
Window->Voiceer->Voice Preset Generatorを選択します。
![](https://user-images.githubusercontent.com/31283418/72231861-08479e80-3601-11ea-80f7-62ec8d60b182.png)

ここでは、既存のファイルをロード、または、新規作成を選べます。
![](https://user-images.githubusercontent.com/31283418/72231859-08479e80-3601-11ea-916c-b9ea6f917a88.png)

### 新規作成
1. 新規作成を押します。

2. 画面が開きます。

![New](https://user-images.githubusercontent.com/31283418/72231860-08479e80-3601-11ea-84d3-d92deb58e24e.png)

3. [+][-]ボタンで、任意のフックでボイスを追加したり消したりします。

![](https://user-images.githubusercontent.com/31283418/72231986-bd7a5680-3601-11ea-8f46-fec58664c17f.png)

4. 終わりです。(自動セーブされているので注意)

### 既存ファイルのロード
1. 一番上行の○をクリックして選択します。

2. 新規作成のとき同様、[+][-]ボタンで、任意のフックでボイスを追加したり消したりします。

3. 終わりです。(自動セーブされているので注意)

## その他
### Preset選択モードに戻る
Presetの選択モードに戻ります。(編集画面が初期化されます。)

# ボイスのパッケージを出力する場合(ボイス製作者さまへ)
1. Window->Voiceer->Voice Preset Generatorを選択します。
2. Presetを選択します。
3. Windowの一番下に[パッケージを出力する]というボタンがあるのでこれをクリックします。
4. 終わりです。

## ボイスファイルについて
* ボイスはAssets/Voiceer/VoiceResources/{プリセット名}以下に置いておくと、みんなが幸せになれます。
* wavファイルを推奨しています。
