# Don-chan Action Theater 証拠台帳

作成日：2026-09-05（UTC）  
対象：Don-chan Action Theater 0.5.0 prototype

## 1. 目的

Action Theaterの着想、実装、試験、公開物を相互参照できる状態にする。ハッシュ値はファイルの同一性確認に用いる。ハッシュ値だけで考案者や作成日時を証明するものではないため、保存履歴、コミット、記事、動画と組み合わせる。

## 2. 原本ZIP

| 項目 | 値 |
|---|---|
| ファイル名 | `DonChanActionTheater-0.5.0-prototype.zip` |
| SHA-256 | `42f696998bc75678e92df6a3e63d56c61974a47a2ec03985fbba013f850549ee` |
| サイズ | 20,690,928 bytes |
| 保存版番号 | 9 |
| 保存作成日時 | 2026-09-03T07:50:09Z |
| 保存更新日時 | 2026-09-04T09:32:38Z |
| ZIP内部最新時刻 | 2026-09-04 17:23 |
| ZIP完全性 | エラーなし |
| エントリ数 | 207 |
| 実ファイル数 | 145 |
| animation.json数 | 28 |
| PNG数 | 104 |

## 3. 主要ファイルのSHA-256

| ファイル | SHA-256 |
|---|---|
| `DonChanActionTheater.dll` | `5bebebafcd3bc9bee69da9b54d293c2ccc05b61e948b938ee61824b0d048ada4` |
| `README.md` | `391f33e09a45d07b0c7ef6f7a24b47ade4b1a1650147d5b72c071d2a0caaa253` |
| `ModInfo.xml` | `70a35e0402c70d16d293af4e0d78cd45e35d2f4d4a85a76e519112a549a61b4b` |
| `Scripts/ActionPatches.cs` | `8524da41b986adda09a73c042d113fe5550992047db38312ad695b9b71fa464d` |
| `Scripts/ActionEvent.cs` | `c687055967daf44b5a5c14cea65b066fd70da7da67d40e9049b8cbd64770693a` |
| `Scripts/ActionTheaterRuntime.cs` | `faeafd1d18cb360c01fbe68e1af84c936d6cdd1adefbfed257aacad70ed9e00d` |
| `Scripts/AnimationLibrary.cs` | `e4b7f6a8bab5ab489d9fe97b428aab0a326ad8335308693d043bb25a7c2983df` |
| `Config/theater.json` | `4a638af99f08f78c11fa55f42b43029655d15030180b7b3674ae00aa18c68696` |

## 4. 同時期の関連記録

| 記録 | 保存位置 | 版 | 内容 |
|---|---|---:|---|
| 原典記録 | `/Action_Theater_原典記録_2026-09-05.md` | 初版 | 定義、背景、実装、発展可能性 |
| 実機テスト | `/7DTD Mod/開発資料/7DTD_2MOD_実機テストリスト.md` | 3 | 0.4.3の操作別試験、2MOD連携、既知不具合 |
| 実装ZIP | `/7DTD Mod/最新版/DonChanActionTheater-0.5.0-prototype.zip` | 9 | ソース、DLL、設定、28演目、104フレーム |
| 画像素材群 | `/画像/7DTD Mod_アクション素材/` | 複数 | 行動別アニメーション素材 |

## 5. コードから確認できる事実

- 入力キーではなくゲーム内部のメソッドへHarmony Patchしている。
- 食事・治療は`consume`完了後に発火する。
- 射撃は`onHoldingEntityFired`後に発火し、空撃ちや拒否された射撃を避ける設計である。
- クラフトは`outputStack`の成功結果を確認する。
- 乗車は`AttachedToEntity`が対象車両と一致したことを確認する。
- `ItemActionMelee`と`ItemActionDynamicMelee`の二系統へ対応する。
- 意味分類は`ActionTopic`と武器・車両サブタイプに分かれる。
- 画像追加はディレクトリ規約とJSON設定で行い、通常はDLL再ビルドを必要としない。
- 連続入力、表示優先度、直前候補回避、Genericフォールバック、設定永続化を備える。

## 6. バージョン順に確認できる発展

README内の変更記録から、少なくとも次の順序が確認できる。各番号の正式公開日時は未確定である。

1. `0.2.0`：飲水、救急、木工、槍、ライフル、ショットガン
2. `0.3.0`：5種類の車両への乗車成立
3. `0.4.0`：スレッジ、ナイフ、斧、シャベル、SMG
4. `0.4.1`：ツルハシを斧より先に判定して分離
5. `0.4.2`：V3.2の二つの近接処理へ対応
6. `0.4.3`：生活系と近接系の再生テンポを調整
7. `0.5.0`：連続入力、チェーンソー、Magnum、Desert Vulture、表示位置・倍率・設定UI

## 7. 公開後に追記するID

| 公開物 | URL / ID | 公開日時 | 対応ハッシュ・版 |
|---|---|---|---|
| GitHubリポジトリ作成 | `https://github.com/itachitachi2-art/DonChanActionTheater` | 2026-09-05T18:17:59Z | `bdecb6c4fee4087ed59d6a4ae0456dacaf8138f9` |
| 原本ZIP公開コミット | `https://github.com/itachitachi2-art/DonChanActionTheater/commit/ea06e140e838567bf560eb664e97ff5b790ceba7` | 2026-09-05 | 0.5.0 / ZIP SHA-256 |
| ソース・証拠文書コミット | 公開後追記 |  |  |
| GitHub Release | 未公開 |  | 0.5.0 / ZIP SHA-256 |
| Zenn記事 | 未公開 |  |  |
| 動作動画 | 未公開 |  | 0.5.0 |
| Web Archive等 | 未実施 |  |  |

## 8. 主張の境界

本台帳から確認できるのは、保存日時までに具体的な名称、コード、DLL、素材、試験仕様、バージョン履歴を備えた実装が存在したことである。

本台帳だけでは、世界中に先行事例が存在しないこと、法的な発明者性、特許性、著作権以外の独占権を証明しない。「世界初」などの表現は、別途先行事例調査を行うまで使用しない。
