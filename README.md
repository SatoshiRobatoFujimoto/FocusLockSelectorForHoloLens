# FocusLockSelector
* Unity 2017.1.1f1
* HoloToolkit-Unity-v1.2017.1.1
* Visual Studio 2017 15.3.3
## 概要
一度Gazeでオブジェクトをフォーカスすると、その後フォーカスを外してもオブジェクトが選択された状態を維持します。
## 使い方
* FocusInputManager.prefabをシーンに配置します。
* フォーカスをロックしたいオブジェクトに、IFocusLockableインタフェースを実装したスクリプトをアタッチします。
* HoloToolkit-Unityの各種入力系のイベントと共存できます。
## 詳細
[http://blog.d-yama7.com/archives/1141](http://blog.d-yama7.com/archives/1141)