# Suman — Unity RPG 冒险游戏

## 项目信息
- **引擎**：Unity 2022 LTS（推荐）或 Unity 6
- **语言**：C#
- **视角**：2D 俯视角（Top-down）

---

## 核心系统一览

| 系统 | 文件 | 说明 |
|------|------|------|
| 游戏管理 | `Core/GameManager.cs` | 单例，管理全局状态机 |
| 场景切换 | `Core/SceneLoader.cs` | 带过渡动画的异步加载 |
| 存档读档 | `Core/SaveSystem.cs` | JSON 文件存档 |
| 玩家属性 | `Player/PlayerStats.cs` | HP/MP/升级等 |
| 玩家控制 | `Player/PlayerController.cs` | 移动 + 交互检测 |
| 战斗系统 | `Combat/CombatManager.cs` | 回合制战斗 |
| 对话系统 | `Dialogue/DialogueManager.cs` | 打字机效果 + 分支选择 |
| 对话数据 | `Dialogue/DialogueData.cs` | ScriptableObject 资产 |
| 背包管理 | `Inventory/InventoryManager.cs` | 叠加 + 增删查 |
| 物品定义 | `Inventory/Item.cs` | ScriptableObject 物品 |
| 剧情任务 | `Story/StoryManager.cs` | 任务追踪 + 剧情标记 |
| UI 管理  | `UI/UIManager.cs` | HUD + 面板开关 |

---

## 快捷键（探索模式）

| 键 | 功能 |
|----|------|
| WASD / 方向键 | 移动 |
| Z / Enter | 交互 / 推进对话 |
| I | 打开/关闭背包 |
| Q | 打开/关闭任务 |
| Esc | 暂停菜单 |

---

## 推荐场景结构

```
Scenes/
├── MainMenu.unity      # 主菜单（新游戏/继续/退出）
├── Village.unity       # 初始村庄
├── Dungeon_01.unity    # 第一关地下城
└── BattleScene.unity   # 战斗场景（可选：独立战斗场景）
```

---

## 下一步建议

1. 在 Unity 中创建工程，将 `Assets/` 文件夹导入
2. 安装 **TextMeshPro** 包（对话/UI 使用）
3. 在场景中创建空对象，挂载 `GameManager`（并关联各子系统）
4. 用 `DialogueData` ScriptableObject 编写第一段 NPC 对话
5. 设计地图并添加 `PlayerController` 预制体
