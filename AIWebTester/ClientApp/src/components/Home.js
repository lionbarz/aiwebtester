import React, {useState} from 'react';

export function Home() {
    const [weatherData, setWeatherData] = useState();

    function getData() {
        setWeatherData("It will be warm on Sunday!");
    }
      
    return (
        <div>
            <p>Are you a website developer? Do you need to know that it's up and running? Stop writing tests! It doesn't get easier than this! Just put in the URL of your website and SmartProbe does the rest! It even uses state of the art AI to come up with tests that you wouldn't have thought of!</p>
            <h1 role='heading' aria-level='1'>National Weather Service</h1>
            <label htmlFor='locationInput'>Find your local forecast</label>
            <input id='locationInput' type='text' className="mx-1"/>
            <button type='submit' className='btn btn-primary' onClick={getData}>Go</button>
            <p>{weatherData}</p>
        </div>
    );
  
}
