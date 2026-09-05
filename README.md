# Don-chan Action Theater

Don-chan Action Theater is a streaming-oriented game mod and presentation format that converts completed in-game player actions into short character performances.

最初の実装対象は **7 Days to Die V3.2** です。食事、治療、クラフト完了、近接攻撃、実射撃、車両への乗車をゲーム内部で検出し、AIキャラクター「どんちゃん」の短いアニメーションとして画面上に表示します。

## Concept

> ゲーム内で成立したプレイヤー行動を意味のあるイベントとして識別し、別のキャラクターによる短い演技へ変換して、視聴者へ可愛く伝える。

Action Theaterはキー入力表示ではありません。

- 左クリックではなく「斧を振った」と判定する
- 使用ボタンではなく「食事が成立した」と判定する
- 射撃入力ではなく「実際に弾が発射された」と判定する
- 乗車操作ではなく「プレイヤーが車両へ接続された」と判定する

取得した意味を、キャラクター固有の演目へ変換します。

## Why it exists

イタチの配信では、操作主体と表現主体が分かれています。

- イタチはゲームを操作し、小さなフェレットとして画面内に存在する
- AIキャラクター「どんちゃん」が台詞と大きな身体表現を担当する
- 視聴者には二人が一緒にゲーム世界を旅しているように届ける

ゲーム内行動とどんちゃんの存在を接続するため、プレイヤーの行動をどんちゃんの短い芝居へ変換する仕組みとしてAction Theaterを制作しました。

## Verified prototype

| Item | Value |
|---|---|
| Mod version | 0.5.0 prototype |
| Target | 7 Days to Die V3.2 |
| Framework | .NET Framework 4.8 / Harmony |
| Animation variants | 28 |
| PNG frames | 104 |
| Archive entries | 207 |
| Actual files | 145 |
| ZIP SHA-256 | `42f696998bc75678e92df6a3e63d56c61974a47a2ec03985fbba013f850549ee` |

The ZIP passed an archive integrity test with no errors.

## Download the preserved prototype

Download `DonChanActionTheater-0.5.0-prototype.zip` from the repository root. It contains the complete installable prototype: source, compiled DLL, configuration, 28 animation definitions, and 104 PNG frames.

The C# source and key project files are also kept directly in the repository for review. The full animation asset tree is preserved inside the hash-verified ZIP.

## Event bridge

| Meaning | Game hook | Guard condition |
|---|---|---|
| Food / Healing | `ItemActionEat.consume` | Fires after the consume step |
| Melee | `ItemActionMelee.ExecuteAction` | Local player, press rather than release |
| Dynamic melee | `ItemActionDynamicMelee.ExecuteAction` | Covers the second V3.2 melee path |
| Ranged | `ItemActionRanged.onHoldingEntityFired` | Fires only after an actual shot |
| Craft | `XUiC_RecipeStack.outputStack` | Requires successful output |
| Vehicle | `EntityVehicle.EnterVehicle` | Requires actual attachment to the vehicle |

Harmony patches feed a semantic `ActionTopic` and subtype into the runtime. The runtime selects an animation, controls priority and repeated input, and draws frames through Unity GUI.

## Included action vocabulary

### Topics

- `Food`
- `Healing`
- `Craft`
- `Melee`
- `Ranged`
- `Vehicle`

### Melee subtypes

`Knife`, `Spear`, `Sledge`, `Baton`, `Axe`, `Pickaxe`, `Shovel`, `Chainsaw`, `Fist`, `Club`, `Generic`

### Ranged subtypes

`Pistol`, `Magnum`, `DesertVulture`, `Rifle`, `Shotgun`, `SMG`, `Bow`, `Crossbow`, `Launcher`, `Generic`

### Vehicle subtypes

`Bicycle`, `Minibike`, `Motorcycle`, `Jeep`, `Gyrocopter`, `Generic`

Unknown or modded items can fall back to `Generic`.

## Adding an animation

No DLL rebuild is required for ordinary animation additions.

```text
Resources/Animations/<Topic>/<Subtype>/<AnimationName>/
  000.png
  001.png
  002.png
  animation.json
```

Example configuration:

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

Multiple variants use weighted random selection. The immediately previous variant is avoided when possible. Repeated or held input does not restart the current performance from its first frame.

## Controls

- `dat on`
- `dat off`
- `dat reload`
- `dat test food`
- `dat test healing`
- `dat test craft`
- `dat test melee [type]`
- `dat test ranged [type]`
- `dat test vehicle [type]`

The Esc menu contains display scale, horizontal position, vertical position, enable/disable, and layout reset controls. Layout settings persist between launches.

## Scope and safety

- Local visual presentation only
- Does not alter player stats, damage, loot, or world rules
- Requires EAC to be disabled because it uses Harmony
- Prototype classification currently relies mainly on internal item names
- Compatibility must be retested after game updates

## Authorship and record

- Concept and creative direction: **イタチ / itachi__don**
- Implementation collaboration: **Itachi & Ai**
- First target implementation: **7 Days to Die V3.2**
- Preserved prototype: **0.5.0**

This repository documents an independently developed expression format. It does not claim verified worldwide priority. The accompanying evidence ledger distinguishes preserved facts from interpretation and future plans.

## License

No public reuse license has been selected yet. Until a license is added, source code and assets remain all rights reserved. Code and asset licensing may be separated in a future release.
