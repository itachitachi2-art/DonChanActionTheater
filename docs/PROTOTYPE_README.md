# Don-chan Action Theater 0.5.0 prototype

7 Days to Die V3.2向け、ローカル表示だけの演出MODです。食事、治療、クラフト完了、近接攻撃、射撃、車両への乗車成立を検出して、画面右中央より少し上にどんちゃんのアニメーションを表示します。ゲーム数値には触れません。

## 導入

`DonChanActionTheater` フォルダを `7 Days To Die/Mods/` に置きます。Harmonyを使うためEAC無効で起動してください。

## テストコマンド

- `dat test food`
- `dat test healing`
- `dat test craft`
- `dat test melee`
- `dat test ranged`
- `dat test melee spear`
- `dat test melee sledge`
- `dat test melee knife`
- `dat test melee axe`
- `dat test melee pickaxe`
- `dat test melee shovel`
- `dat test melee chainsaw`
- `dat test ranged rifle`
- `dat test ranged shotgun`
- `dat test ranged smg`
- `dat test ranged magnum`
- `dat test ranged desertvulture`
- `dat test vehicle bicycle`
- `dat test vehicle minibike`
- `dat test vehicle motorcycle`
- `dat test vehicle jeep`
- `dat test vehicle gyrocopter`
- `dat on` / `dat off`
- `dat reload`（画像追加後にゲームを再起動せず再読込）

## アニメーション追加

次の形でフォルダを追加し、`dat reload` を実行します。DLLの再ビルドは不要です。

```text
Resources/Animations/<題材>/<武器種>/<好きなアニメ名>/
  000.png
  001.png
  002.png
  animation.json  （省略可能）
```

題材は `Food`, `Healing`, `Craft`, `Melee`, `Ranged`, `Vehicle`。武器種や車種が不要なら `Generic` を使います。画像は透過PNG、ファイル名の昇順で再生されます。

```json
{
  "fps": 10.0,
  "loop": false,
  "holdLastFrame": 0.18,
  "weight": 1.0,
  "displaySeconds": 0.0,
  "repeatCount": 1
}
```

同じ題材・武器種に複数のアニメフォルダがあれば重み付きランダム再生します。直前と同じ候補は可能なら避けます。射撃などの連続動作は `sequenceLockSeconds` の間、最初に選ばれたアニメへ固定されます。

試作には28アニメーション、合計104フレームを収録しています。Food、Healing、Craft、Pistol、Melee/Genericでは複数候補のランダム選択を確認できます。連続入力中は選択されたアニメーションを固定します。

### 0.2.0追加素材

- Food/Generic: 飲水4フレーム
- Healing/Generic: 救急箱と包帯4フレーム
- Craft/Generic: 木工作業4フレーム
- Melee/Spear: 槍突き4フレーム
- Ranged/Rifle: ライフル射撃4フレーム
- Ranged/Shotgun: 射撃・反動・ポンプ操作4フレーム

### 0.3.0追加素材

- Vehicle/Bicycle: 自転車への乗車4フレーム
- Vehicle/Minibike: ミニバイクへの乗車4フレーム
- Vehicle/Motorcycle: バイクへの乗車4フレーム
- Vehicle/Jeep: 4×4への乗車4フレーム
- Vehicle/Gyrocopter: ジャイロコプターへの乗車4フレーム

乗車ボタンを押しただけでは表示せず、ローカルプレイヤーが実際に車両へ接続された後に発火します。分類できないMOD車両は `Vehicle/Generic` へフォールバックできます。

### 0.4.0追加素材

- Melee/Sledge: スレッジハンマー4フレーム
- Melee/Knife: ナイフ4フレーム
- Melee/Axe: 斧4フレーム
- Melee/Shovel: シャベル4フレーム
- Ranged/SMG: SMG射撃4フレーム

シャベルは `shovel` / `spade` を独立判定します。

### 0.4.1追加素材

- Melee/Pickaxe: ツルハシ採掘4フレーム

内部名の `pickaxe` を `axe` より先に判定するため、ツルハシと斧を分離して表示します。

### 0.4.2近接検出修正

V3.2の近接攻撃処理は `ItemActionMelee` と `ItemActionDynamicMelee` の2系統があります。旧版で未接続だったDynamic側を追加し、バニラ武器と従来型・MOD武器の両方で、命中の有無にかかわらず攻撃入力時に演出を起動します。

### 0.4.3再生テンポ調整

- 近接アニメーションを6～8fpsへ落とし、短時間のループ再生へ変更しました。押しっぱなしでも先頭だけでなく全フレームを追いやすくなります。
- 射撃アニメーションの速度と連続射撃時の動きは変更していません。
- 食事、治療、クラフト、乗車の同梱アニメーションを3fps・約2.2秒表示へ変更しました。

## 試作版の分類

- 近接: Knife / Spear / Sledge / Baton / Axe / Pickaxe / Shovel / Chainsaw / Fist / Club / Generic
- 射撃: Pistol / Magnum / DesertVulture / Rifle / Shotgun / SMG / Bow / Crossbow / Launcher / Generic
- MOD武器や分類不能な武器: Genericへフォールバック

分類は内部アイテム名を基にした初版です。実機ログと表示結果を見てV3.2用のタグ判定へ詰めます。

### 0.5.0操作・連続入力調整

- 長押し・連打による同一アクションの追加入力では、再生中のアニメーションを先頭へ戻さず最後まで継続します。終了後も入力が続いていれば同じ候補で次の再生を開始します。
- 食事、治療、クラフトは同じアニメーションを2回再生します。
- チェーンソーを近接へ、.44 MagnumとDesert Vultureを専用の射撃サブタイプへ分類します。
- 標準表示を従来の1.5倍にし、右中央より少し上へ移動しました。
- Escメニュー右下の設定パネルから、表示倍率・横位置・縦位置・ON/OFFを変更できます。位置と倍率は保存されます。
