﻿ (*
 * The MIT License
 *
 * Copyright 2018 The ALANN2018 authors.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *)

module Params

// NAL Related Parameters
let HORIZON                             = 1.0f          // System Personality Factor
                                                        
// Node Related Parameters                              
let DECAY_RATE                          = 0.75f         // Lambda decay rate for node forgetting - higher value -> slower decay
let ACTIVATION_THRESHOLD                = 0.70f         // Minimum concept STI for concept activation
let LATENCY_PERIOD                      = 2L            // Concept latency period in milliseconds
let BELIEF_CAPACITY                     = 200           // Max number of beliefs per node *** WORKSTATION ***
let INFERENCE_CONFIDENCE_MIN            = 0.30f         // Confidence must reach this value before it can be used in general inference

// Temporal Related Parameters
let CONCURRENCY_DURATION                = 50L           // Period when two occurence times are deemed concurrent
let TEMPORAL_DISCOUNT                   = 0.5f          // Used by sequencer to discount temporal truth function
let PAST_TENSE_OFFSET                   = 100L          // Time before now when past tense occured (ms)
let FUTURE_TENSE_OFFSET                 = 100L          // Time after now when future tense occured (ms)
let ASSUMPTION_OF_FAILURE_PENALTY       = 0.1f          // Amount to reduce predictive hypotheses conf by

// General Parameters
let CONFIDENCE                          = 0.90f         // Truth Value confidence component
let FREQUENCY                           = 1.00f         // Truth Value frequency component
let MINIMUM_CONFIDENCE                  = 0.10f         // don't accept inference results with confidence below this Value
let MINIMUM_STI                         = 0.05f         // filter STI below this threhold
let STI                                 = 0.50f         // Short Term Importance default Value AKA priority
let LTI                                 = 0.50f         // long Term Importance default Value AKA duration
let NODE_STI                            = 0.75f         // Initial Node Short Term Importance default Value AKA priority
let NODE_LTI                            = 0.50f         // Initial Node long Term Importance default Value AKA duration
let USER_STI                            = 0.70f         // Short Term Importance default Value for user entered Values AKA priority
let USER_LTI                            = 0.90f         // long Term Importance default Value for user entered Values AKA duration
let TRAIL_LENGTH                        = 15            // maximum length allowed for inference trail within stamp
let MAX_SC                              = 100           // Maximum syntactic complexity of terms
let BUFFER_SELECTION_FACTOR             = 0.3f          // Determines the curve slope of the priority buffer selection
let TERM_DEPTH                          = 3             // depth of term separation
let ATTENTION_BUFFER_SIZE               = 10            // Maximum number of events in Attention buffer
let INPUT_BUFFER_SIZE                   = 1_000         // Maximum number of events in input buffer

// UI related Parameters
let NODES_PROCESSED_MOD                 = 1_000_000     // Frequency of display of processd nodes
let NODE_COUNT_MOD                      = 10_000        // Frequency of display of created node count
let EVENTS_PROCESSED_MOD                = 200_000L      // Frequency of display of selected events 
let INFERENCES_PROCESSED_MOD            = 100_000L      // Frequency of display of derived events

//Streams related Parameters    ***WORKSTATION SETUP***
let WORKSTATION                         = true          // Set to true if running on a workstation rather than a server
let NUM_TERM_STREAMS                    = 10             // Number of Term streams
let NUM_SELECTED_BELIEFS                = 10             // Number of beliefs selected after attention allocated in concept
let CYCLE_DELAY_MS                      = 5.0           // Number of ms to allow for main cycle delay
let GROUP_DELAY_MS                      = 5.0           // Number of ms to allow for grouping of events before despatching 
let MAJOR_BLOCK_SIZE                    = 10_000        // Number of events to form a main stream block
let MINOR_BLOCK_SIZE                    = 1_000         // Number of events to form a minor stream block
