# 喝水提醒小助手 - Windows 桌面应用

<div align="center">

💧 一款简洁美观的 Windows 喝水提醒应用，帮助您养成规律饮水的好习惯

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![WPF](https://img.shields.io/badge/WPF-UI-512BD4)
![Platform](https://img.shields.io/badge/Platform-Windows-0078D4)
![License](https://img.shields.io/badge/License-MIT-green)

</div>

## ✨ 功能特性

- 🎯 **每日目标追踪** - 设置每日饮水目标，实时显示完成进度
- ⏰ **智能定时提醒** - 自定义提醒间隔和时间段，Windows 原生 Toast 通知
- 📊 **数据统计图表** - 7/14/30 天饮水统计，直观了解饮水习惯
- 🚀 **快捷记录** - 一键记录常用饮水量，支持自定义数量
- 💾 **本地数据存储** - SQLite 本地数据库，数据安全私密
- 🎨 **现代界面** - Fluent Design 设计风格，支持浅色/深色主题
- 🖼️ **系统托盘** - 常驻托盘，最小化到托盘不打扰工作

## 📸 应用截图

> 截图待添加

## 🛠️ 技术栈

| 组件 | 技术 |
|------|------|
| 运行时 | .NET 8 (LTS) |
| UI 框架 | WPF + WPF-UI |
| 系统托盘 | Hardcodet.NotifyIcon.Wpf |
| 数据库 | SQLite |
| 图表 | LiveCharts2 |
| MVVM | CommunityToolkit.Mvvm |
| 通知 | Windows Community Toolkit |

## 📦 安装

### 前置要求

- Windows 10/11 (64-bit)
- [.NET 8 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

### 从源码构建

1. 克隆仓库
```bash
git clone https://github.com/your-username/drink-reminder.git
cd drink-reminder
```

2. 安装 .NET SDK 8.0（如果尚未安装）
- 下载地址: https://dotnet.microsoft.com/download/dotnet/8.0

3. 构建项目
```bash
dotnet restore
dotnet build --configuration Release
```

4. 运行应用
```bash
dotnet run --project DrinkReminder/DrinkReminder.csproj
```

### 发布独立应用

```bash
dotnet publish DrinkReminder/DrinkReminder.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## 📁 项目结构

```
drink-reminder/
├── DrinkReminder.sln                 # 解决方案文件
├── DrinkReminder/                    # 主项目
│   ├── App.xaml(.cs)                 # 应用入口
│   ├── MainWindow.xaml(.cs)          # 主窗口
│   ├── Models/                       # 数据模型
│   │   ├── DrinkRecord.cs            # 饮水记录
│   │   ├── DailyStats.cs             # 每日统计
│   │   └── AppSettings.cs            # 应用设置
│   ├── Services/                     # 服务层
│   │   ├── DatabaseService.cs        # 数据库操作
│   │   ├── ReminderService.cs        # 定时提醒
│   │   └── TrayService.cs            # 托盘管理
│   ├── ViewModels/                   # 视图模型
│   │   ├── MainViewModel.cs          # 主窗口 VM
│   │   ├── HomeViewModel.cs          # 首页 VM
│   │   ├── HistoryViewModel.cs       # 历史页 VM
│   │   └── SettingsViewModel.cs      # 设置页 VM
│   ├── Views/                        # 视图
│   │   ├── HomeView.xaml(.cs)        # 首页
│   │   ├── HistoryView.xaml(.cs)     # 历史页
│   │   └── SettingsView.xaml(.cs)    # 设置页
│   ├── Converters/                   # 值转换器
│   ├── Helpers/                      # 辅助类
│   └── Resources/                    # 资源文件
└── README.md
```

## ⚙️ 配置

应用设置存储在:
```
%APPDATA%\DrinkReminder\drinkreminder.db
```

### 默认设置

| 设置项 | 默认值 |
|--------|--------|
| 每日目标 | 2000 ml |
| 提醒间隔 | 30 分钟 |
| 提醒时段 | 08:00 - 22:00 |
| 快捷记录按钮 | 200ml, 300ml, 500ml |

## 🔧 开发

### NuGet 依赖

```xml
<PackageReference Include="WPF-UI" Version="3.0.5" />
<PackageReference Include="Hardcodet.NotifyIcon.Wpf" Version="1.1.0" />
<PackageReference Include="Microsoft.Data.Sqlite" Version="8.0.11" />
<PackageReference Include="LiveChartsCore.SkiaSharpView.WPF" Version="2.0.0-rc2" />
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.3.2" />
<PackageReference Include="CommunityToolkit.WinUI.Notifications" Version="7.1.2" />
```

## 📝 待办事项

- [ ] 添加应用图标
- [ ] 实现数据导出功能 (CSV)
- [ ] 添加饮水量统计报表
- [ ] 支持多语言
- [ ] 添加安装程序 (MSIX/Inno Setup)
- [ ] 自动更新功能

## 📄 许可证

本项目基于 MIT 许可证开源 - 详见 [LICENSE](LICENSE) 文件

## 🤝 贡献

欢迎提交 Issue 和 Pull Request！

---

<div align="center">

**💧 记得多喝水哦！💧**

</div>