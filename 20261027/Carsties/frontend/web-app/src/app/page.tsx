import Listings from "@/features/listings/Listings";
import {AppPagination} from "@/components/ui/app-pagination";
import {getListings} from "@/features/listings/actions";

// PageProps reloads the page every time the URL changes
export default async function Home(props: PageProps<"/">) {
    const searchParams = await props.searchParams;
    
    const page = Number(searchParams["pageNumber"] || 1);
    const pageSize = Number(searchParams["pageSize"] || 8);
    
    const data = await getListings(page);
    
    return (
        <div>
            <Listings auctions={data.results}/>
            <AppPagination page={page} pageSize={pageSize} totalCount={data.totalCount} />
        </div>
      );
}
