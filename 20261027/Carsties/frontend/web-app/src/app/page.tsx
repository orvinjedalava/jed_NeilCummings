import Listings from "@/features/listings/Listings";
import {AppPagination} from "@/components/ui/app-pagination";
import {getListings} from "@/features/listings/actions";

// PageProps reloads the page every time the URL changes
export default async function Home(props: PageProps<"/">) {
    const searchParams = await props.searchParams;
    
    const data = await getListings(searchParams);
    
    return (
        <div>
            <Listings auctions={data.results}/>
            <AppPagination 
                page={Number(searchParams["pageNumber"] || 1)} 
                pageSize={Number(searchParams["pageSize"] || 8)} 
                totalCount={data.totalCount} 
            />
        </div>
      );
}
