import { getListingDetails } from "@/features/listings/actions";
import NotFound from "next/dist/client/components/builtin/not-found";

export default async function ListingDetailedPage(props: PageProps<"/listings/[id]">) {
    const { id } = await props.params;
    const auction = await getListingDetails(id);
    
    if (!auction) return NotFound();
    
    return (
        <div>{auction.make} {auction.model}</div>
    );
}