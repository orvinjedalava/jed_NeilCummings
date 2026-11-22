import {Auction, PagedResult} from "@/lib/types";

export type ListingSearchParams = {
    pageNumber?: string | string[];
    pageSize?: string | string[];
    searchTerm?: string | string[];
}

export async function getListings(params: ListingSearchParams = {}): Promise<PagedResult<Auction>> {
    const { pageNumber, pageSize, searchTerm } = params;
    const query = new URLSearchParams({
        pageNumber: pageNumber?.toString() || String(1),
        pageSize: pageSize?.toString() || String(8),
    });
    
    if (searchTerm)
        query.set("searchTerm", searchTerm.toString());
    
    const res = await fetch(
        `http://localhost:6001/search?${query}`);

    if (!res.ok) throw new Error('Failed to fetch data');

    return res.json(); // returns the body of the response
}