import {Auction, PagedResult} from "@/lib/types";

export async function getListings(pageNumber = 1, pageSize = 4, searchTerm?: string | string[]): Promise<PagedResult<Auction>> {
    const query = new URLSearchParams({
        pageNumber: pageNumber.toString(),
        pageSize: pageSize.toString(),
    });
    
    if (searchTerm)
        query.set("searchTerm", searchTerm.toString());
    
    const res = await fetch(
        `http://localhost:6001/search?${query}`);

    if (!res.ok) throw new Error('Failed to fetch data');

    return res.json(); // returns the body of the response
}