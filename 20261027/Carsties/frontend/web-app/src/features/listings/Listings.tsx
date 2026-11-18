import AuctionCard from "@/features/listings/AuctionCard";
import {getListings} from "@/features/listings/actions";

export default async function Listings() {
    const data = await getListings();
    
    return (
        <div className={'grid grid-cols-4 gap-6'}>
            {data.results.map((auction) => (
                <AuctionCard auction={auction} key={auction.id} />
            ))}
        </div>
    );
}