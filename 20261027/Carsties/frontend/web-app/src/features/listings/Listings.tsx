import AuctionCard from "@/features/listings/AuctionCard";

async function getListings() {
    const res = await fetch(
        'http://localhost:6001/search');
    
    if (!res.ok) throw new Error('Failed to fetch data');
    
    return res.json(); // returns the body of the response
}

export default async function Listings() {
    const data = await getListings();
    
    return (
        <div className={'grid grid-cols-4 gap-6'}>
            {data.results.map((auction:any) => (
                <AuctionCard auction={auction} key={auction.id} />
            ))}
        </div>
    );
}