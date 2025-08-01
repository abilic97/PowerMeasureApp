// This file can be replaced during build by using the `fileReplacements` array.
// `ng build` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  API_URL: 'https://localhost:5001',
  SOCKET_URL: 'https://sockets.diggipiggy.xyz',
  graphTheme : {
  
    color: [
      '#c12e34','#e6b600','#0098d9','#2b821d',
      '#005eaa','#339ca8','#cda819','#32a487'
  ],
  
    title: {
        textStyle: {
            fontWeight: 'normal'
        }
    },
  
    visualMap: {
        color:['#1790cf','#a2d4e6']
    },
  
    toolbox: {
        iconStyle: {
            normal: {
                borderColor: '#06467c'
            }
        }
    },
  
    tooltip: {
        backgroundColor: '#ffffff'
    },
  
    dataZoom: {
        dataBackgroundColor: '#dedede',
        fillerColor: 'rgba(154,217,247,0.2)',
        handleColor: '#005eaa'
    },
  
    timeline: {
        lineStyle: {
            color: '#005eaa'
        },
        controlStyle: {
            normal: {
                color: '#005eaa',
                borderColor: '#005eaa'
            }
        }
    },
  
    candlestick: {
        itemStyle: {
            normal: {
                color: '#c12e34',
                color0: '#2b821d',
                lineStyle: {
                    width: 1,
                    color: '#c12e34',
                    color0: '#2b821d'
                }
            }
        }
    },
  
    graph: {
        color: [
          '#c12e34','#e6b600','#0098d9','#2b821d',
          '#005eaa','#339ca8','#cda819','#32a487'
      ]
    },
  
    map: {
        label: {
            normal: {
                textStyle: {
                    color: '#c12e34'
                }
            },
            emphasis: {
                textStyle: {
                    color: '#c12e34'
                }
            }
        },
        itemStyle: {
            normal: {
                borderColor: '#eee',
                areaColor: '#ddd'
            },
            emphasis: {
                areaColor: '#e6b600'
            }
        }
    },
  
    gauge: {
        axisLine: {
            show: true,
            lineStyle: {
                color: [[0.2, '#2b821d'],[0.8, '#005eaa'],[1, '#c12e34']],
                width: 5
            }
        },
        axisTick: {
            splitNumber: 10,
            length:8,
            lineStyle: {
                color: 'auto'
            }
        },
        axisLabel: {
            textStyle: {
                color: 'auto'
            }
        },
        splitLine: {
            length: 12,
            lineStyle: {
                color: 'auto'
            }
        },
        pointer: {
            length: '90%',
            width: 3,
            color: 'auto'
        },
        title: {
            textStyle: {
                color: '#333'
            }
        },
        detail: {
            textStyle: {
                color: 'auto'
            }
        }
    }
  }
};



/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/plugins/zone-error';  // Included with Angular CLI.
