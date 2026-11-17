async function getListings() {
    const res = await fetch(
        'http://localhost:6001/search');
    
    if (!res.ok) throw new Error('Failed to fetch data');
    
    return res.json(); // returns the body of the response
}

export default async function Listings() {
    const data = await getListings();
    
    return (
        <div>
            {JSON.stringify(data.results, null, 2)}
        </div>
    );
}