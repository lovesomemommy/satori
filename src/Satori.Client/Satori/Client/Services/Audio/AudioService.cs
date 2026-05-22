using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Satori.Client.State;
using Satori.Core.Models.Settings;

namespace Satori.Client.Services.Audio;

public sealed class AudioService : IAudioService
{
	private const float FadeSpeed = 1.5f;

	private LocalTrackManifest _manifest = new LocalTrackManifest();

	private string _tracksDirectory = string.Empty;

	private GameSettingsModel? _settings;

	private Song? _loadedSong;

	private string _loadedTrackId = string.Empty;

	private bool _musicRequested;

	private float _fadeTarget;

	private float _fadeCurrent;

	private bool _loopPending;

	public void Initialize()
	{
		_tracksDirectory = ResolveTracksDirectory();
		_manifest = LocalTrackManifest.LoadFromDirectory(_tracksDirectory);
		MediaPlayer.IsRepeating = true;
	}

	public void ApplySettings(GameSettingsModel settings)
	{
		_settings = settings;
		_fadeTarget = settings.IsMuted ? 0f : settings.MasterVolume;
		if (!_musicRequested)
		{
			_fadeCurrent = _fadeTarget;
		}

		ApplyMediaVolume();
		if (_musicRequested && !string.IsNullOrEmpty(_loadedTrackId) && _loadedTrackId != settings.SelectedMusicTrackId)
		{
			PlayTrack(settings.SelectedMusicTrackId, fadeIn: true);
		}
		else if (_musicRequested)
		{
			ResumeIfNeeded();
		}
	}

	public void Update(GameTime gameTime)
	{
		if (_settings == null)
		{
			return;
		}

		float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (Math.Abs(_fadeCurrent - _fadeTarget) <= 0.001f)
		{
			_fadeCurrent = _fadeTarget;
		}
		else if (_fadeCurrent < _fadeTarget)
		{
			_fadeCurrent = Math.Min(_fadeTarget, _fadeCurrent + FadeSpeed * delta);
		}
		else
		{
			_fadeCurrent = Math.Max(_fadeTarget, _fadeCurrent - FadeSpeed * delta);
		}

		ApplyMediaVolume();
		if (_loopPending && _fadeTarget > 0f)
		{
			_loopPending = false;
			StartPlayback();
		}
	}

	public void OnSceneChanged(GameStateType state)
	{
		if (state == GameStateType.Boot)
		{
			StopMusic(fadeOut: false);
			return;
		}

		EnsureMusicPlaying();
	}

	public void PlayTrack(string trackId, bool fadeIn = true)
	{
		if (_settings == null)
		{
			return;
		}

		string path = _manifest.ResolveTrackFilePath(_tracksDirectory, trackId);
		if (string.IsNullOrEmpty(path) || !File.Exists(path))
		{
			_musicRequested = false;
			_loadedTrackId = string.Empty;
			_loopPending = false;
			DisposeSong();
			MediaPlayer.Stop();
			_fadeCurrent = 0f;
			_fadeTarget = 0f;
			return;
		}

		bool sameTrack = string.Equals(_loadedTrackId, trackId, StringComparison.OrdinalIgnoreCase);
		if (!sameTrack)
		{
			DisposeSong();
			try
			{
				_loadedSong = Song.FromUri(Path.GetFileName(path), new Uri(path, UriKind.Absolute));
				_loadedTrackId = trackId;
				StartPlayback();
			}
			catch (Exception)
			{
				_musicRequested = false;
				_loadedTrackId = string.Empty;
				_loopPending = false;
				DisposeSong();
				MediaPlayer.Stop();
				_fadeCurrent = 0f;
				_fadeTarget = 0f;
				return;
			}
		}
		else
		{
			ResumeIfNeeded();
		}

		_musicRequested = true;
		_settings.SelectedMusicTrackId = trackId;
		_fadeTarget = _settings.IsMuted ? 0f : _settings.MasterVolume;
		if (!fadeIn)
		{
			_fadeCurrent = _fadeTarget;
		}

		ApplyMediaVolume();
	}

	public void StopMusic(bool fadeOut = true)
	{
		_musicRequested = false;
		_loopPending = false;
		_fadeTarget = 0f;
		if (!fadeOut)
		{
			_fadeCurrent = 0f;
			MediaPlayer.Stop();
			ApplyMediaVolume();
		}
	}

	public void PreviewCurrentTrack()
	{
		if (_settings == null)
		{
			return;
		}

		PlayTrack(_settings.SelectedMusicTrackId, fadeIn: true);
	}

	private void EnsureMusicPlaying()
	{
		if (_settings == null)
		{
			return;
		}

		string trackId = _settings.SelectedMusicTrackId;
		if (string.IsNullOrWhiteSpace(trackId) || FindExistingTrackPath(trackId) == null)
		{
			trackId = _manifest.DefaultTrackId;
		}

		if (string.IsNullOrWhiteSpace(trackId) || FindExistingTrackPath(trackId) == null)
		{
			return;
		}

		_fadeTarget = _settings.IsMuted ? 0f : _settings.MasterVolume;
		if (_musicRequested &&
		    string.Equals(_loadedTrackId, trackId, StringComparison.OrdinalIgnoreCase) &&
		    _loadedSong != null &&
		    MediaPlayer.State == MediaState.Playing)
		{
			ApplyMediaVolume();
			return;
		}

		PlayTrack(trackId, fadeIn: true);
	}

	private void ResumeIfNeeded()
	{
		if (!_musicRequested || _loadedSong == null || _fadeTarget <= 0f)
		{
			return;
		}

		if (MediaPlayer.State != MediaState.Playing)
		{
			StartPlayback();
		}
	}

	private string? FindExistingTrackPath(string trackId)
	{
		string path = _manifest.ResolveTrackFilePath(_tracksDirectory, trackId);
		return !string.IsNullOrEmpty(path) && File.Exists(path) ? path : null;
	}

	private void ApplyMediaVolume()
	{
		MediaPlayer.Volume = Math.Clamp(_fadeCurrent, 0f, 1f);
	}

	private void StartPlayback()
	{
		if (_loadedSong == null)
		{
			return;
		}

		if (MediaPlayer.State == MediaState.Playing)
		{
			return;
		}

		if (MediaPlayer.State == MediaState.Paused)
		{
			MediaPlayer.Resume();
			return;
		}

		MediaPlayer.IsRepeating = true;
		MediaPlayer.Play(_loadedSong);
	}

	private void DisposeSong()
	{
		_loadedSong = null;
	}

	private static string ResolveTracksDirectory()
	{
		return Path.Combine(AppContext.BaseDirectory, "LocalTracks");
	}
}
