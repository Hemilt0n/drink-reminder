using System.IO;
using Microsoft.Data.Sqlite;
using DrinkReminder.Models;

namespace DrinkReminder.Services;

/// <summary>
/// 数据库服务 - 管理SQLite数据库操作
/// </summary>
public class DatabaseService : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly string _dbPath;
    private bool _disposed;

    public DatabaseService()
    {
        // 数据库文件存储在应用数据目录
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appDataPath, "DrinkReminder");

        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }

        _dbPath = Path.Combine(appFolder, "drinkreminder.db");
        var connectionString = $"Data Source={_dbPath}";

        _connection = new SqliteConnection(connectionString);
        _connection.Open();

        Initialize();
    }

    /// <summary>
    /// 初始化数据库表结构
    /// </summary>
    private void Initialize()
    {
        using var command = _connection.CreateCommand();

        // 创建饮水记录表
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS DrinkRecords (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                AmountMl INTEGER NOT NULL,
                Timestamp TEXT NOT NULL,
                Note TEXT
            )";
        command.ExecuteNonQuery();

        // 创建设置表
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Settings (
                Key TEXT PRIMARY KEY,
                Value TEXT NOT NULL
            )";
        command.ExecuteNonQuery();

        // 创建索引以提高查询性能
        command.CommandText = @"
            CREATE INDEX IF NOT EXISTS IX_DrinkRecords_Timestamp
            ON DrinkRecords (Timestamp)";
        command.ExecuteNonQuery();
    }

    #region 饮水记录操作

    /// <summary>
    /// 添加饮水记录
    /// </summary>
    public long AddRecord(int amountMl, string? note = null)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO DrinkRecords (AmountMl, Timestamp, Note)
            VALUES (@amount, @timestamp, @note);
            SELECT last_insert_rowid();";

        command.Parameters.AddWithValue("@amount", amountMl);
        command.Parameters.AddWithValue("@timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        command.Parameters.AddWithValue("@note", note ?? (object)DBNull.Value);

        var result = command.ExecuteScalar();
        return result != null ? Convert.ToInt64(result) : 0;
    }

    /// <summary>
    /// 删除饮水记录
    /// </summary>
    public void DeleteRecord(long id)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "DELETE FROM DrinkRecords WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// 获取今日统计
    /// </summary>
    public DailyStats GetTodayStats()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var records = GetRecords(today, tomorrow);
        var settings = GetSettings();

        return new DailyStats
        {
            Date = today,
            TotalMl = records.Sum(r => r.AmountMl),
            GoalMl = settings.DailyGoalMl,
            Records = records
        };
    }

    /// <summary>
    /// 获取指定日期的统计
    /// </summary>
    public DailyStats GetDailyStats(DateTime date)
    {
        var startOfDay = date.Date;
        var nextDay = startOfDay.AddDays(1);

        var records = GetRecords(startOfDay, nextDay);
        var settings = GetSettings();

        return new DailyStats
        {
            Date = startOfDay,
            TotalMl = records.Sum(r => r.AmountMl),
            GoalMl = settings.DailyGoalMl,
            Records = records
        };
    }

    /// <summary>
    /// 获取历史记录
    /// </summary>
    public List<DailyStats> GetHistory(int days = 7)
    {
        var result = new List<DailyStats>();
        var settings = GetSettings();

        for (int i = 0; i < days; i++)
        {
            var date = DateTime.Today.AddDays(-i);
            var stats = GetDailyStats(date);
            stats.GoalMl = settings.DailyGoalMl;
            result.Add(stats);
        }

        return result;
    }

    /// <summary>
    /// 获取指定时间范围内的记录
    /// </summary>
    private List<DrinkRecord> GetRecords(DateTime start, DateTime end)
    {
        var records = new List<DrinkRecord>();

        using var command = _connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, AmountMl, Timestamp, Note
            FROM DrinkRecords
            WHERE Timestamp >= @start AND Timestamp < @end
            ORDER BY Timestamp DESC";

        command.Parameters.AddWithValue("@start", start.ToString("yyyy-MM-dd HH:mm:ss"));
        command.Parameters.AddWithValue("@end", end.ToString("yyyy-MM-dd HH:mm:ss"));

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            records.Add(new DrinkRecord
            {
                Id = reader.GetInt64(0),
                AmountMl = reader.GetInt32(1),
                Timestamp = DateTime.Parse(reader.GetString(2)),
                Note = reader.IsDBNull(3) ? null : reader.GetString(3)
            });
        }

        return records;
    }

    /// <summary>
    /// 获取最近N条记录
    /// </summary>
    public List<DrinkRecord> GetRecentRecords(int count = 10)
    {
        var records = new List<DrinkRecord>();

        using var command = _connection.CreateCommand();
        command.CommandText = @"
            SELECT Id, AmountMl, Timestamp, Note
            FROM DrinkRecords
            ORDER BY Timestamp DESC
            LIMIT @count";

        command.Parameters.AddWithValue("@count", count);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            records.Add(new DrinkRecord
            {
                Id = reader.GetInt64(0),
                AmountMl = reader.GetInt32(1),
                Timestamp = DateTime.Parse(reader.GetString(2)),
                Note = reader.IsDBNull(3) ? null : reader.GetString(3)
            });
        }

        return records;
    }

    /// <summary>
    /// 清除所有数据
    /// </summary>
    public void ClearAllData()
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "DELETE FROM DrinkRecords";
        command.ExecuteNonQuery();
    }

    #endregion

    #region 设置操作

    /// <summary>
    /// 获取应用设置
    /// </summary>
    public AppSettings GetSettings()
    {
        var settings = new AppSettings();

        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT Key, Value FROM Settings";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var key = reader.GetString(0);
            var value = reader.GetString(1);

            ApplySetting(settings, key, value);
        }

        return settings;
    }

    /// <summary>
    /// 保存应用设置
    /// </summary>
    public void SaveSettings(AppSettings settings)
    {
        var settingsDict = new Dictionary<string, string>
        {
            { "DailyGoalMl", settings.DailyGoalMl.ToString() },
            { "ReminderIntervalMinutes", settings.ReminderIntervalMinutes.ToString() },
            { "ReminderEnabled", settings.ReminderEnabled.ToString().ToLower() },
            { "ReminderStartTime", settings.ReminderStartTime.ToString() },
            { "ReminderEndTime", settings.ReminderEndTime.ToString() },
            { "QuickRecordButtons", string.Join(",", settings.QuickRecordButtons) },
            { "AutoStart", settings.AutoStart.ToString().ToLower() },
            { "MinimizeToTray", settings.MinimizeToTray.ToString().ToLower() },
            { "CloseToTray", settings.CloseToTray.ToString().ToLower() },
            { "Theme", settings.Theme }
        };

        using var transaction = _connection.BeginTransaction();

        foreach (var kvp in settingsDict)
        {
            using var command = _connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO Settings (Key, Value)
                VALUES (@key, @value)";

            command.Parameters.AddWithValue("@key", kvp.Key);
            command.Parameters.AddWithValue("@value", kvp.Value);
            command.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    /// <summary>
    /// 应用设置值到对象
    /// </summary>
    private void ApplySetting(AppSettings settings, string key, string value)
    {
        try
        {
            switch (key)
            {
                case "DailyGoalMl":
                    settings.DailyGoalMl = int.Parse(value);
                    break;
                case "ReminderIntervalMinutes":
                    settings.ReminderIntervalMinutes = int.Parse(value);
                    break;
                case "ReminderEnabled":
                    settings.ReminderEnabled = bool.Parse(value);
                    break;
                case "ReminderStartTime":
                    settings.ReminderStartTime = TimeSpan.Parse(value);
                    break;
                case "ReminderEndTime":
                    settings.ReminderEndTime = TimeSpan.Parse(value);
                    break;
                case "QuickRecordButtons":
                    settings.QuickRecordButtons = value.Split(',')
                        .Select(s => int.Parse(s.Trim()))
                        .ToList();
                    break;
                case "AutoStart":
                    settings.AutoStart = bool.Parse(value);
                    break;
                case "MinimizeToTray":
                    settings.MinimizeToTray = bool.Parse(value);
                    break;
                case "CloseToTray":
                    settings.CloseToTray = bool.Parse(value);
                    break;
                case "Theme":
                    settings.Theme = value;
                    break;
            }
        }
        catch
        {
            // 忽略无效的设置值
        }
    }

    #endregion

    public void Dispose()
    {
        if (!_disposed)
        {
            _connection.Close();
            _connection.Dispose();
            _disposed = true;
        }
    }
}