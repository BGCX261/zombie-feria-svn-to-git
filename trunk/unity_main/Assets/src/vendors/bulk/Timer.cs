using UnityEngine;
using System.Collections;

/// <summary>
/// My timer : Initialize a timer.
/// </summary>
public class Timer
{
	protected	float	__beginTime = 0.0f;
	protected	float	__endTime = 0.0f;
	protected	bool	__paused = true;
	protected	float	__pausedTime = 0.0f;
	
	public Timer()
	{
	}
	
	/// <summary>
	/// Start the specified time.
	/// </summary>
	/// <param name='time'>
	/// Time.
	/// </param>
	public	void	start(float _time)
	{
		this.__beginTime = Time.time;
		this.__endTime = Time.time + _time;
		this.__paused = false;
	}
	
	/// <summary>
	/// Pause the timer.
	/// </summary>
	public	void	pause()
	{
		if ( this.__paused == false )
		{
			this.__pausedTime = Time.time;
			this.__paused = true;
		}
	}
	/// <summary>
	/// Continue the timer.
	/// </summary>
	public	void	goOn()
	{
		if ( this.__paused )
		{
			this.__endTime = this.__endTime + (Time.time - this.__pausedTime);
			this.__paused = false;
		}
	}
	
	/**
	 * Return the elapsed time
	 * */
	public	float	getElapsedTime()
	{
		return (Time.time - this.__beginTime );	
	}
	
	/// <summary>
	/// Determines whether the  timer is finished.
	/// </summary>
	/// <returns>
	/// <c>true</c> if the timer is finished; otherwise, <c>false</c>.
	/// </returns>
	public	bool	isFinished()
	{
		if ( this.__paused == false )
		{
			if( Time.time > this.__endTime )
			{
				return true;
			}
		}
		return false;
	}
	
	/// <summary>
	/// Return the actual time at wich the timer has to end.
	/// </summary>
	/// <returns>
	/// The end time.
	/// </returns>
    public	float	getEndTime()
	{
        return this.__endTime;
    }
	
	/// <summary>
	/// Return the remaining time until the timer's end.
	/// </summary>
	/// <returns>
	/// The remaining time.
	/// </returns>
	public	float	getRemainingTime()
	{
		return (this.__endTime - Time.time);
	}
	
	public	bool	IsPaused()
	{
		return this.__paused;
	}
}
