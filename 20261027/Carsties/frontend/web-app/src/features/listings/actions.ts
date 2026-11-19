import {Auction, PagedResult} from "@/lib/types";

export async function getListings(pageNumber = 1, pageSize = 4): Promise<PagedResult<Auction>> {
    const res = await fetch(
        `http://localhost:6001/search?pageNumber=${pageNumber}&pageSize=${pageSize}`);

    if (!res.ok) throw new Error('Failed to fetch data');

    return res.json(); // returns the body of the response
}