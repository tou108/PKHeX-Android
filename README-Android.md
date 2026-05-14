# PKHeX Android (MAUI)

PKHeX.Core をそのまま使用し、.NET MAUI で構築した Android 版 PKHeX です。

## プロジェクト構成

```
PKHeX-Android/
├── PKHeX.Core/              ← 元の PKHeX.Core (net10.0 + net10.0-android)
├── PKHeX.Android/           ← MAUI Android アプリ
│   ├── Pages/               ← 画面 (XAML)
│   │   ├── MainPage         ← ホーム画面
│   │   ├── SaveEditorPage   ← セーブデータ編集
│   │   ├── BoxViewPage      ← ボックス表示
│   │   └── PokemonEditorPage ← ポケモン編集
│   ├── ViewModels/          ← MVVM ViewModel
│   ├── Services/            ← SaveFileService (PKHeX.Core ラッパー)
│   └── Converters/          ← XAML Converters
└── PKHeX.Android.sln        ← ソリューション
```

## 機能

- ✅ セーブデータ読み込み (全世代対応、PKHeX.Core が処理)
- ✅ トレーナー情報編集 (名前、所持金)
- ✅ ボックス表示 (6×N グリッド)
- ✅ 手持ち表示
- ✅ ポケモン編集 (IV/EV/技/性格/色違い)
- ✅ MAX ステータス一括設定
- ✅ 合法性チェック (PKHeX.Core の LegalityAnalysis)
- ✅ 合法化 (Legalize())
- ✅ セーブデータエクスポート (共有シート)

## ビルド方法

### 必要なもの

| ツール | バージョン |
|--------|-----------|
| Visual Studio 2022 または Rider | 最新版 |
| .NET SDK | 10.0 以上 |
| .NET MAUI ワークロード | `dotnet workload install maui-android` |
| Android SDK | API Level 26 以上 |
| Java JDK | 17 以上 |

### セットアップ

```bash
# MAUI Android ワークロードのインストール
dotnet workload install maui-android

# Android SDK のインストール確認
dotnet workload list
```

### ビルド

```bash
cd PKHeX-Android

# Debug ビルド (実機/エミュレータにデプロイ)
dotnet build PKHeX.Android/PKHeX.Android.csproj -f net10.0-android

# Release APK 生成
dotnet publish PKHeX.Android/PKHeX.Android.csproj \
  -f net10.0-android \
  -c Release \
  -p:AndroidPackageFormat=apk \
  -p:AndroidKeyStore=false
```

APK は `PKHeX.Android/bin/Release/net10.0-android/` に生成されます。

### Visual Studio でのビルド

1. `PKHeX.Android.sln` を Visual Studio で開く
2. `PKHeX.Android` をスタートアッププロジェクトに設定
3. ターゲットを `net10.0-android` に設定
4. ビルド → Android デバイス/エミュレータで実行

## 署名 (リリース配布)

```bash
# キーストア生成
keytool -genkey -v -keystore pkhex-release.keystore \
  -alias pkhex -keyalg RSA -keysize 2048 -validity 10000

# 署名付き APK
dotnet publish PKHeX.Android/PKHeX.Android.csproj \
  -f net10.0-android \
  -c Release \
  -p:AndroidPackageFormat=apk \
  -p:AndroidKeyStore=true \
  -p:AndroidSigningKeyStore=pkhex-release.keystore \
  -p:AndroidSigningKeyAlias=pkhex \
  -p:AndroidSigningKeyPass=YOUR_KEY_PASS \
  -p:AndroidSigningStorePass=YOUR_STORE_PASS
```

## 設計上の注意点

### なぜ WinForms をそのまま使えないか

| コンポーネント | ターゲット | Android 対応 |
|--------------|-----------|-------------|
| PKHeX.Core | net10.0 | ✅ 完全対応 |
| PKHeX.Drawing | net10.0-windows | ❌ System.Drawing.Common は Windows 専用 |
| PKHeX.WinForms | net10.0-windows | ❌ Windows Forms は Android 非対応 |

### 解決方法

- **PKHeX.Core**: `TargetFrameworks` に `net10.0-android` を追加するだけで OK
- **描画**: ポケモンスプライトが必要な場合は `SkiaSharp` または `Microsoft.Maui.Graphics` を使用
- **UI**: WinForms の代わりに MAUI XAML で Android ネイティブ UI を構築

## ライセンス

PKHeX.Core は GPL-3.0 ライセンスです。このプロジェクトも同様に GPL-3.0 に従います。
