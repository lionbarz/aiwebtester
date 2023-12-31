import React, {useState} from 'react';

export function Probe() {
    const [url, setUrl] = useState('http://www.google.com');
    const [loading, setLoading] = useState(false);
    const [probeResult, setProbeResult] = useState();
    const [error, setError] = useState(undefined);
    const startProbe = async () => {
        setLoading(true);
        setError(undefined);
        setProbeResult(undefined);
        try {
            const response = await fetch('test?url=' + url);
            const data = await response.json();
            setProbeResult(data);
            setError(undefined);
        } catch(e) {
            console.error(e);
            setProbeResult(undefined);
            setError(e);
        }
        setLoading(false);
    };
    
    return (
        <div>
            <h1>Prober</h1>
            <form>
                <label>URL to probe:</label>
                <input type='text' value={url} onChange={(e) => setUrl(e.target.value)}/>
            </form>
            <button onClick={startProbe}>Go!</button>
            {loading && <div>Loading...</div>}
            {probeResult && 
                <div>
                    <div>
                        Action: {probeResult?.action?.explain}
                    </div>
                    <div>
                        Before: <img width="100%" src={"data:image/png;base64," + probeResult?.beforeScreenshotBytes} />
                    </div>
                    <div>
                        After: <img width="100%" src={"data:image/png;base64," + probeResult?.afterScreenshotBytes} />
                    </div>
                    <div>
                        Expected: {probeResult?.expected}
                    </div>
                </div>}
        </div>
    );
}
