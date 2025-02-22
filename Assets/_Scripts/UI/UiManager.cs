﻿using Bladiator.Managers;
using TMPro;
using UnityEngine;

namespace Bladiator.UI
{
	public class UiManager : MonoBehaviour
	{
		public static UiManager Instance;

		[SerializeField] private TMP_Text m_WaveText;
		[SerializeField] private Animator m_Idle, m_WaveDone, m_AnnouncementObject, m_FightObject, m_Transition;

		private void Awake()
		{
			if (Instance != null)
			{
				Destroy(gameObject);
				return;
			}

			Instance = this;
		}

		private void Start()
		{
			GameManager.Instance.OnGameStateChange += OnGameStateChange;
			GameManager.Instance.ResetEvent += ResetEvent;

			if (GameManager.Instance.GameState == GameState.Idle)
				m_Idle.Play("Idle");
		}

		private void ResetEvent()
		{
			m_AnnouncementObject.Play("Idle_State");
			m_FightObject.Play("Idle_State");
			m_Idle.Play("Idle_State");
		}

		private void OnGameStateChange(GameState obj)
		{
			if (obj == GameState.Animating)
			{
				// Reset the idle animator to the idle state
				m_Idle.Play("Idle_State");

				// Update the Wave text
				m_WaveText.text = $"WAVE {WaveSystem.Instance.WaveCount}";

				// Play the announcement
				m_AnnouncementObject.Play("Wave_Announcement");
			}
			else if (obj == GameState.Idle)
			{
				// Resetting all animators to their default idle state
				m_AnnouncementObject.Play("Idle_State");
				m_WaveDone.Play("Idle_State");
				m_FightObject.Play("Idle_State");
				m_Idle.Play("Idle_State");

				// Start the idle animator
				m_Idle.Play("Idle");
			} else if (obj == GameState.Pause)
			{
				// Resetting all animators to their default idle state
				m_AnnouncementObject.Play("Idle_State");
				m_WaveDone.Play("Idle_State");
				m_FightObject.Play("Idle_State");
				m_Idle.Play("Idle_State");
			} else if (obj == GameState.Ending)
			{
				m_Transition.Play("Begin");
			} else if (obj == GameState.MainMenu)
			{
				m_Transition.Play("End");
			}
		}

		public void ShowWaveDone()
		{
			m_WaveDone.Play("Wave_Done");
		}
		
		public void ShowFight()
		{
			m_FightObject.Play("Fight");
		}

		public void AnimationDone()
		{
			WaveSystem.Instance.StartSpawn();
			GameManager.Instance.AnimationDone();
		}
	}
}