﻿using UnityEngine;
using CRI.HitBoxTemplate.Serial;

namespace CRI.HitBoxTemplate.Example
{
	public class MovingBar : MonoBehaviour
	{
		[SerializeField] private GameObject _barUp;
		[SerializeField] private GameObject _barDown;
		private GameObject _barUsed;
		[SerializeField] private Material _red;
		[SerializeField] private Material _blue;
		[SerializeField] private float _speed0 = .5f;
		private float _speed = .5f;
		[SerializeField] private float _randomSpeedFactor = .5f;
		[SerializeField] private float _maxAngle = 45f;
		[SerializeField] private float _delay2Hit0 = 3f;
		[SerializeField] private float _rangeDelay = 1f;
		private float _delay2Hit = 3f;
		public float currentAngle = 0f;
		private int _direction = 1;
		private float _timeKeeper = 0f;
		private float _timeSinceStrike = 0f;
		public float reactionTime = 0f;
		private bool _lineMoving = true;
		private bool _strikeTime = true;
		private bool _started = false;
		public int round = 0;

		[SerializeField]
		private GameObject _hitFeedback;

		private void OnEnable()
		{
			ImpactPointControl.onImpact += OnImpact;
		}

		private void OnDisable()
		{
			ImpactPointControl.onImpact -= OnImpact;
		}

		private void OnImpact(object sender, ImpactPointControlEventArgs e)
		{
			if (_hitFeedback != null) {
				Instantiate(_hitFeedback, new Vector3(e.impactPosition.x, e.impactPosition.y, 30), Quaternion.identity);
			}
			SetImpact();
		}

		private void SetImpact() {
			if (!_strikeTime || Time.realtimeSinceStartup < 2f) // waiting for the startup ghost hits to finish before registering any hit
				return;
			if (!_started)
			{
				_started = true;
				round++;
				DataSaver.Instance.Separator("Round Start");
				return;
			}
			if (Random.Range(0, 2) == 0)
				_direction = -_direction;
			_lineMoving = true;
			_barUsed.GetComponent<MeshRenderer>().material = _blue;
			_barUsed.GetComponent<TargetMovingBar>().activated = false;
			_strikeTime = false;
			reactionTime = _timeSinceStrike;

			// Update delay 2 hits
			_delay2Hit = _delay2Hit0 + Random.Range(-_rangeDelay / 2f, _rangeDelay / 2f);
			_speed = _speed0 * Random.Range(1/_randomSpeedFactor, _randomSpeedFactor);
		}

		// Update is called once per frame
		void FixedUpdate()
		{
			currentAngle = (transform.position.x) * 3.6f;
			if (!_started)
				return;
			_timeKeeper += Time.fixedDeltaTime;
			if (_timeKeeper >= 180.0f)
			{
				_started = false;
				_timeKeeper = 0f;
				_timeSinceStrike = 0f;
				DataSaver.Instance.Separator("Round Over");
				return;
			}
			else if (_timeSinceStrike >= _delay2Hit)
			{
				if (Random.Range(0, 2) == 0)
					_barUsed = _barUp;
				else
					_barUsed = _barDown;


				_barUsed.GetComponent<MeshRenderer>().material = _red;
				if (Random.Range(0, 2) == 0)
					_barUsed.GetComponent<TargetMovingBar>().side = 1;
				else
					_barUsed.GetComponent<TargetMovingBar>().side = -1;
				_barUsed.GetComponent<TargetMovingBar>().activated = true;


				_lineMoving = false;
				_timeSinceStrike = 0f;
				_strikeTime = true;
			}
			else if (_lineMoving)
			{
				_timeSinceStrike += Time.fixedDeltaTime;
				float _xMax = 100f / 360f * _maxAngle;
				if (transform.position.x > _xMax || transform.position.x < -_xMax || Input.GetKeyDown(KeyCode.Mouse0))
					_direction = -_direction;
				transform.Translate(_speed * _direction, 0, 0);
			}

			if (Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Space)) {
				SetImpact();
				Debug.Log("Hi");
			}
		}
	}
}