/*
  Cassowary.net: an incremental constraint solver for .NET
  (http://lumumba.uhasselt.be/jo/projects/cassowary.net/)
  
  Copyright (C) 2005-2006  Jo Vermeulen (jo.vermeulen@uhasselt.be)
  
  This program is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public License
  as published by the Free Software Foundation; either version 2.1
  of  the License, or (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;

namespace Cassowary.Tests
{
  /// <summary>
  /// A simple timer class for benchmarking in test cases.
  /// </summary>
  public class Timer
  {
    public Timer()
    {
      _isRunning = false; // start not yet called
    }

    /// <summary>
    /// Start the timer.
    /// </summary>
    public void Start()
    {
      _isRunning = true; // stopwatch is now running
      // look at internal clock and remember reading
      _startReading = DateTime.Now;
    }

    /// <summary>
    /// Stop the timer.
    /// </summary>
    public void Stop()
    {
      _isRunning = false; // stop timer object
      DateTime stopReading = DateTime.Now; // calculate current reading
      // calculate difference
      _elapsedTime = (stopReading - _startReading).TotalSeconds;
    }

    /// <summary>
    /// Clears a Timer of previous elapsed times, so that
    /// a new event can be timed.
    /// </summary>
    public void Reset()
    {
      _isRunning = false; // start not yet called
    }

    /// <summary>
    /// Used to keep track wether a timer is active, i.e. wether an 
    /// event is being timed. 
    /// </summary>
    public bool IsRunning
    {
      get { return _isRunning; }
    }

    /// <summary>
    /// Get the amount of time that elapsed on a timer object.
    /// </summary>
    public double ElapsedTime
    {
      get 
      {
        if (!_isRunning)
          return _elapsedTime;
        else
          return (DateTime.Now - _startReading).TotalSeconds;
      }
    }
    
    private bool _isRunning;
    private double _elapsedTime;
    private DateTime _startReading;
  }
}
