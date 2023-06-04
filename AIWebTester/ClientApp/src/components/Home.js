import React, {Component, useState} from 'react';

export function Home() {
    const [weatherData, setWeatherData] = useState();

    function getData() {
        setWeatherData("It will be warm on Sunday!");
    }
      
    return (
        <div>
            <h1 role='heading' aria-level='1'>National Weather Service</h1>
            <label htmlFor='locationInput'>Find your local forecast</label>
            <input id='locationInput' type='text' className="mx-1"/>
            <button type='submit' className='btn btn-primary' onClick={getData}>Go</button>
            <p>{weatherData}</p>
        </div>
    );
  
}
