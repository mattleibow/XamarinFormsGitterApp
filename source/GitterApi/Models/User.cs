using Newtonsoft.Json;

namespace GitterApi.Models
{
	public class User
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("username")]
		public string Username { get; set; }

		[JsonProperty("displayName")]
		public string DisplayName { get; set; }

		[JsonProperty("url")]
		public string Url { get; set; }

		[JsonProperty("avatarUrlSmall")]
		public string SmallAvatarUrl { get; set; }

		[JsonProperty("avatarUrlMedium")]
		public string MediumAvatarUrl { get; set; }

		[JsonIgnore]
		public string GitHubUrl => $"https://github.com{Url}";
	}

	public class UserProfile
	{
		public UserProfile()
		{
		}

		public UserProfile(User user)
		{
			Id = user.Id;
			Username = user.Username;
			DisplayName = user.DisplayName;
			Profile = user.GitHubUrl;
		}

		[JsonProperty("company")]
		public string Company { get; set; }

		[JsonProperty("displayName")]
		public string DisplayName { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("github")]
		public GitHub GitHub { get; set; }

		[JsonProperty("gv")]
		public string GV { get; set; }

		[JsonProperty("has_gitter_login")]
		public bool HasGitterLogin { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("location")]
		public string Location { get; set; }

		[JsonProperty("profile")]
		public string Profile { get; set; }

		[JsonProperty("username")]
		public string Username { get; set; }

		[JsonProperty("website")]
		public string Website { get; set; }
	}

	public class GitHub
	{
		[JsonProperty("followers")]
		public int Followers { get; set; }

		[JsonProperty("public_repos")]
		public int PublicRepos { get; set; }

		[JsonProperty("following")]
		public int Following { get; set; }
	}
}
