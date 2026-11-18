import { Card, CardContent } from "@/components/ui/card";
import Link from "next/link";
import Image from "next/image";
import CountdownTimer from "@/features/listings/CountdownTimer";

type Props = {
    auction: any;
}

export default function AuctionCard({ auction }: Props) {
    return (
        <Link href={`/listings/${auction.id}`}>
            <Card className='relative mx-auto w-full pt-0'>
                <Image 
                    src={auction.imageUrl} 
                    alt='Image of car'
                    height={400}
                    width={400}
                    className='object-cover aspect-16/10 border-b border-muted-foreground/20'
                    priority
                    sizes='(max-width:768px) 100vw, (max-width: 1200px) 50vw, 33vw'
                />
                <div className='absolute bottom-18 left-2'>
                    <CountdownTimer auctionEnd={auction.auctionEnd} />
                </div>
                <CardContent className='flex justify-between items-center'>
                    <h3 className='text-muted-foreground'>
                        {auction.make} {auction.model}
                    </h3>
                    <p className='font-semibold text-sm'>{auction.year}</p>
                </CardContent>
            </Card>
        </Link>
        
    );
}