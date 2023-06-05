using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

/*
Supported SI tables
===================

 * Program Allocation Table (PAT)
 * Conditional Access Table (CAT)
 * Transport Stream Descriptor Table (TSDT)
 * Program Map Table (PMT)
 * Network Information Table (NIT)
 * Bouquet Association Table (BAT)
 * Service Definition Table (SDT)
 * Event Information Table (EIT)
 * Time and Date Table (TDT)
 * Time Offset Table (TOT)
 * Running Status Table (RST)
 * Stuffing Table (ST)
 * Discontinuity Information Table (DIT)
 * Selection Information Table (SIT)


Supported MPEG descriptors
==========================

 * Descriptor 0x02: Video stream descriptor
 * Descriptor 0x03: Audio stream descriptor
 * Descriptor 0x04: Hierarchy descriptor
 * Descriptor 0x05: Registration descriptor
 * Descriptor 0x06: Data stream alignment descriptor
 * Descriptor 0x07: Target Background Grid descriptor
 * Descriptor 0x08: Video Window descriptor
 * Descriptor 0x09: Conditional access descriptor
 * Descriptor 0x0A: ISO-639 language descriptor
 * Descriptor 0x0B: System clock descriptor
 * Descriptor 0x0C: Multiplex buffer utilization descriptor
 * Descriptor 0x0D: Copyright descriptor
 * Descriptor 0x0E: Maximum bitrate descriptor
 * Descriptor 0x0F: Private data indicator descriptor
 * Descriptor 0x10: Smoothing buffer descriptor
 * Descriptor 0x11: STD descriptor
 * Descriptor 0x12: IBP descriptor
 * Descriptor 0x1b: MPEG-4 video descriptor
 * Descriptor 0x1c: MPEG-4 audio descriptor
 * Descriptor 0x1d: IOD_descriptor
 * Descriptor 0x1e: SL_descriptor
 * Descriptor 0x1f: FMC_descriptor
 * Descriptor 0x20: External ES_ID descriptor
 * Descriptor 0x21: MuxCode descriptor
 * Descriptor 0x22: FmxBufferSize descriptor
 * Descriptor 0x23: MultiplexBuffer descriptor
 * Descriptor 0x24: Content labeling descriptor
 * Descriptor 0x25: Metadata pointer descriptor
 * Descriptor 0x26: Metadata descriptor
 * Descriptor 0x27: Metadata STD descriptor
 * Descriptor 0x28: AVC video descriptor
 * Descriptor 0x2a: AVC timing and HRD descriptor
 * Descriptor 0x2b: MPEG-2 AAC audio descriptor
 * Descriptor 0x2c: FlexMuxTiming_descriptor


Supported DVB descriptors
=========================

 * Descriptor 0x40: Network name descriptor
 * Descriptor 0x41: Service list descriptor
 * Descriptor 0x42: Stuffing descriptor
 * Descriptor 0x43: Satellite delivery system descriptor
 * Descriptor 0x44: Cable delivery system descriptor
 * Descriptor 0x45: VBI data descriptor
 * Descriptor 0x46: VBI teletext descriptor
 * Descriptor 0x47: Bouquet name descriptor
 * Descriptor 0x48: Service descriptor
 * Descriptor 0x49: Country availability descriptor
 * Descriptor 0x4a: Linkage descriptor
 * Descriptor 0x4b: NVOD reference descriptor
 * Descriptor 0x4c: Time shifted service descriptor
 * Descriptor 0x4d: Short event descriptor
 * Descriptor 0x4e: Extended event descriptor
 * Descriptor 0x4f: Time shifted event descriptor
 * Descriptor 0x50: Component descriptor
 * Descriptor 0x51: Mosaic descriptor
 * Descriptor 0x52: Stream identifier descriptor
 * Descriptor 0x53: CA identifier descriptor
 * Descriptor 0x54: Content descriptor
 * Descriptor 0x55: Parental rating descriptor
 * Descriptor 0x56: Teletext descriptor
 * Descriptor 0x57: Telephone descriptor
 * Descriptor 0x58: Local time offset descriptor
 * Descriptor 0x59: Subtitling descriptor
 * Descriptor 0x5a: Terrestrial delivery system descriptor
 * Descriptor 0x5b: Multilingual network name descriptor
 * Descriptor 0x5c: Multilingual bouquet name descriptor
 * Descriptor 0x5d: Multilingual service name descriptor
 * Descriptor 0x5e: Multilingual component descriptor
 * Descriptor 0x5f: Private data specifier descriptor
 * Descriptor 0x60: Service move descriptor
 * Descriptor 0x61: Short smoothing buffer descriptor
 * Descriptor 0x62: Frequency list descriptor
 * Descriptor 0x63: Partial transport stream descriptor
 * Descriptor 0x64: Data broadcast descriptor
 * Descriptor 0x65: Scrambling descriptor
 * Descriptor 0x66: Data broadcast id descriptor
 * Descriptor 0x67: Transport stream descriptor
 * Descriptor 0x68: DSNG descriptor
 * Descriptor 0x69: PDC descriptor
 * Descriptor 0x6a: AC-3 descriptor
 * Descriptor 0x6b: Ancillary data descriptor
 * Descriptor 0x6c: Cell list descriptor
 * Descriptor 0x6d: Cell frequency link descriptor
 * Descriptor 0x6e: Announcement support descriptor
 * Descriptor 0x7a: Enhanced AC-3 descriptor
 * Descriptor 0x7b: DTS descriptor
 * Descriptor 0x7c: AAC descriptor


 * >> VIDEO_STREAM_MPEG1 = 0x01,
>> VIDEO_STREAM_MPEG2 = 0x02,
>> AUDIO_STREAM_MPEG1 = 0x03, // all layers including mp3
>> AUDIO_STREAM_MPEG2 = 0x04,
>> VIDEO_STREAM_H264 = 0x1b,
>> AUDIO_STREAM_LPCM = 0x80,
>> AUDIO_STREAM_AC3 = 0x81,
>> AUDIO_STREAM_DTS = 0x82,
>> -> AUDIO_STREAM_AC3_TRUE_HD = 0x83,
>> -> AUDIO_STREAM_AC3_PLUS = 0x84,
>> -> AUDIO_STREAM_DTS_HD = 0x85,
>> -> AUDIO_STREAM_DTS_HD_MASTER_AUDIO = 0x86,
>> PRESENTATION_GRAPHICS_STREAM = 0x90,
>> INTERACTIVE_GRAPHICS_STREAM = 0x91,
>> SUBTITLE_STREAM = 0x92,
>> SECONDARY_AUDIO_AC3_PLUS = 0xa1,
>> SECONDARY_AUDIO_DTS_HD = 0xa2,
>> VIDEO_STREAM_VC1 = 0xea
 */

namespace DVDSlideshow.BluRay.BDMV
{
    /// <summary>
    /// This abstract class represents a BDMV descriptor
    /// </summary>
    public abstract class Descriptor
    {
        private byte tag;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="idtag"></param>
        public Descriptor(byte IdTag)
        {
            this.tag = IdTag;
        }

        /// <summary>
        /// Base write
        /// </summary>
        /// <param name="writer"></param>
        public virtual void Write(BinaryWriter writer)
        {
            writer.Write(tag);
        }

        /// <summary>
        /// Abtsract method to get the length of the descriptor in bytes
        /// </summary>
        /// <returns></returns>
        public abstract uint GetLengthInBytes();  
    }
}
