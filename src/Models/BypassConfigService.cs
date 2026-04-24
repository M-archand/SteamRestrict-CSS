using Newtonsoft.Json;

namespace SteamRestrict
{
	public class PlayerBypassConfig
	{
		public bool BypassMinimumCS2Level { get; set; } = false;
		public bool BypassMinimumHours { get; set; } = false;
		public bool BypassMinimumLevel { get; set; } = false;
		public bool BypassMinimumSteamAccountAge { get; set; } = false;
		public bool BypassPrivateProfile { get; set; } = false;
		public bool BypassUnconfiguredProfile { get; set; } = false;
		public bool BypassTradeBanned { get; set; } = false;
		public bool BypassVACBanned { get; set; } = false;
		public bool BypassSteamGroupCheck { get; set; } = false;
		public bool BypassGameBanned { get; set; } = false;
	}

	public class BypassConfig
	{
		private Dictionary<ulong, PlayerBypassConfig> _playerConfigs = new Dictionary<ulong, PlayerBypassConfig>();

		public PlayerBypassConfig? GetPlayerConfig(ulong steamID)
		{
			if (_playerConfigs.TryGetValue(steamID, out var playerConfig))
				return playerConfig;

			return null;
		}

		public void AddPlayerConfig(ulong steamID, PlayerBypassConfig playerConfig)
		{
			_playerConfigs[steamID] = playerConfig;
		}

		public Dictionary<ulong, PlayerBypassConfig> GetAllPlayerConfigs()
		{
			return _playerConfigs;
		}
	}

	public class BypassConfigService
	{
		private readonly string _configFilePath;

		public BypassConfigService(string configFilePath)
		{
			_configFilePath = configFilePath;
		}

		public BypassConfig LoadConfig()
		{
			if (File.Exists(_configFilePath))
			{
				string json = File.ReadAllText(_configFilePath);
				var playerConfigs = JsonConvert.DeserializeObject<Dictionary<ulong, PlayerBypassConfig>>(json);
				if (playerConfigs == null)
					return new BypassConfig();

				var bypassConfig = new BypassConfig();

				foreach (var kvp in playerConfigs)
				{
					bypassConfig.AddPlayerConfig(kvp.Key, kvp.Value);
				}

				return bypassConfig;
			}
			else
			{
				var defaultConfig = new BypassConfig();

				defaultConfig.AddPlayerConfig(76561197960434622, new PlayerBypassConfig
				{
					BypassMinimumCS2Level = true,
					BypassMinimumHours = false,
					BypassMinimumLevel = true,
					BypassMinimumSteamAccountAge = false,
					BypassPrivateProfile = true,
					BypassTradeBanned = false,
					BypassVACBanned = true,
					BypassSteamGroupCheck = false,
					BypassGameBanned = true
				});

				defaultConfig.AddPlayerConfig(76561197985607672, new PlayerBypassConfig
				{
					BypassMinimumCS2Level = false,
					BypassMinimumHours = true,
					BypassMinimumLevel = false,
					BypassMinimumSteamAccountAge = true,
					BypassPrivateProfile = false,
					BypassTradeBanned = true,
					BypassVACBanned = false,
					BypassSteamGroupCheck = true,
					BypassGameBanned = false
				});

				string json = JsonConvert.SerializeObject(defaultConfig.GetAllPlayerConfigs(), Formatting.Indented);
				File.WriteAllText(_configFilePath, json);

				return defaultConfig;
			}
		}
	}
}
