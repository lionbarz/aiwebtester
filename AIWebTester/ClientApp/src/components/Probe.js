import React, {useState} from 'react';

export function Probe() {
    const [url, setUrl] = useState('http://www.google.com');
    const [loading, setLoading] = useState(false);
    const [probeResult, setProbeResult] = useState();
    const startProbe = async () => {
        setLoading(true);
        try {
            const response = await fetch('test?url=' + url);
            const data = await response.json();
            setProbeResult(data);
        } catch(e) {
            console.error(e);
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
            <div>
                Action: {probeResult?.action?.explain}
            </div>
            <div>
                Before: <img width="20em" src={"data:image/png;base64," + probeResult?.beforeScreenshotBytes} />
            </div>
            <div>
                After: <img width="20em" src={"data:image/png;base64," + probeResult?.afterScreenshotBytes} />
            </div>
            <div>
                Expected: {probeResult?.expected}
            </div>
        </div>
    );
}
