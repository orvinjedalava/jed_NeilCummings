import { Card } from "@/components/ui/card";

type Props = {
    auction: any;
}

export default function AuctionCard({ auction }: Props) {
    return (
        <Card className='p-4'>
            {auction.make} {auction.model}
        </Card>
    );
}