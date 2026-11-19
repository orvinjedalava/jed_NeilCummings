'use client'

import {
    Pagination,
    PaginationContent,
    PaginationEllipsis,
    PaginationItem,
    PaginationLink,
    PaginationNext,
    PaginationPrevious,
} from "@/components/ui/pagination"
import {usePathname, useRouter, useSearchParams} from "next/navigation";

type Props = {
    page: number,
    pageSize: number,
    totalCount: number
}

export function AppPagination( { page, pageSize, totalCount }: Props) {
    
    const router = useRouter();
    const pathname = usePathname();
    const searchParams = useSearchParams();
    
    const totalPages = Math.ceil(totalCount / pageSize);
    
    const goToPage = (pageNumber: number) => {
        if (pageNumber < 1 || pageNumber > totalPages || pageNumber === page)
            return;
        
        const params = new URLSearchParams(searchParams.toString());
        params.set('pageNumber', pageNumber.toString());
        
        // update the URL after clicking to a page
        router.push(`${pathname}?${params.toString()}`);
    }
    
    const pageNumbers: (number | 'ellipsis')[] = [];
    
    for (let i = 1; i <= totalPages; i++) {
        // always include page #1
        // always include the total pages number
        // include number if it's within 1 of the current page
        if (i === 1 || i === totalPages || Math.abs(i - page) <= 1) {
            pageNumbers.push(i);
        } else if (pageNumbers[pageNumbers.length - 1] !== 'ellipsis') {
            pageNumbers.push('ellipsis');
        }
    }
    
    return (
        <Pagination className='fixed max-w-none inset-x-0 bottom-0 z-50 w-full border-t border-border/70 bg-background/70 py-2 backdrop-blur-2xl'>
            <PaginationContent>
                <PaginationItem>
                    <PaginationPrevious 
                        href="#" 
                        onClick={e => {
                            e.preventDefault();
                            goToPage(page - 1);
                        }}
                        className={ page <= 1 ? 'pointer-events-none opacity-50' : undefined }
                    />
                </PaginationItem>

                {pageNumbers.map((pageNumber, index) => 
                    pageNumber === 'ellipsis' ? (
                        <PaginationItem key={index}>
                            <PaginationEllipsis />
                        </PaginationItem>
                    ) : (
                        <PaginationItem key={index}>
                            <PaginationLink 
                                href="#"
                                isActive={pageNumber === page}
                                onClick={e => { 
                                    e.preventDefault();
                                    goToPage(pageNumber)
                                }}
                            >
                                {pageNumber}
                            </PaginationLink>
                        </PaginationItem>
                    )
                )}
                
                <PaginationItem>
                    <PaginationNext 
                        href="#"
                        onClick={e => {
                            e.preventDefault();
                            goToPage(page + 1);
                        }}
                        className={ page >= totalPages ? 'pointer-events-none opacity-50' : undefined } 
                    />
                </PaginationItem>
            </PaginationContent>
        </Pagination>
    )
}
