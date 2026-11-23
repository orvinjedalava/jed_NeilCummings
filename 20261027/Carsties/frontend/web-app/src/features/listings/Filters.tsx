'use client';

import {ArrowDownAZ, ClockArrowDown, ClockPlus, Flame, Icon, OctagonPause, Timer} from "lucide-react";
import {ButtonGroup} from "@/components/ui/button-group";
import {Button} from "@/components/ui/button";

const orderButtons = [
    { label: 'Alphabetical', icon: ArrowDownAZ, value: 'make'},
    { label: 'End date', icon: ClockArrowDown, value: 'endingSoon'},
    { label: 'Recently added', icon: ClockPlus, value: 'new'}
]

const filterButtons = [
    { label: 'Live Auctions', icon: Flame, value: 'live'},
    { label: 'Ending < 10 hours', icon: OctagonPause, value: 'endingSoon'},
    { label: 'Completed', icon: Timer, value: 'finished'}
]

export default function Filters() {
    return (
        <div className='flex justify-between items-center mb-4 -mt-4 p-4 border-b border-muted-foreground'>
            <div className='text-2xl font-semibold'>
                All listings
            </div>
            <div className='flex items-center gap-3'>
                <span className='uppercase text-sm text-muted-foreground'>Filter by</span>
                <ButtonGroup>
                    {filterButtons.map(({label, icon: Icon, value}) => (
                        <Button key={value} variant='outline'>
                            <Icon />
                            {label}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>
            <div className='flex items-center gap-3'>
                <span className='uppercase text-sm text-muted-foreground'>Order by</span>
                <ButtonGroup>
                    {orderButtons.map(({label, icon: Icon, value}) => (
                        <Button key={value} variant='outline'>
                            <Icon />
                            {label}
                        </Button>
                    ))}
                </ButtonGroup>
            </div>
        </div>
    )
}